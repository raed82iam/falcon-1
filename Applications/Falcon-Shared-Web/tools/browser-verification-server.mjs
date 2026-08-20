import { createServer } from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import { extname, resolve, sep } from 'node:path';
import { fileURLToPath } from 'node:url';

const ROOT = fileURLToPath(new URL('../', import.meta.url));
const HOST = '127.0.0.1';
const DEFAULT_PORT = 4173;
const requestedPort = Number.parseInt(process.env.FALCON_WEB_VERIFY_PORT ?? `${DEFAULT_PORT}`, 10);
const PORT = Number.isInteger(requestedPort) && requestedPort >= 1024 && requestedPort <= 65535
  ? requestedPort
  : DEFAULT_PORT;

const MIME = Object.freeze({
  '.html':'text/html; charset=utf-8',
  '.js':'text/javascript; charset=utf-8',
  '.mjs':'text/javascript; charset=utf-8',
  '.css':'text/css; charset=utf-8',
  '.json':'application/json; charset=utf-8',
  '.svg':'image/svg+xml',
  '.png':'image/png',
  '.jpg':'image/jpeg',
  '.jpeg':'image/jpeg',
  '.webp':'image/webp',
  '.ico':'image/x-icon'
});

function safeFilePath(requestUrl) {
  let pathname;
  try {
    pathname = decodeURIComponent(new URL(requestUrl ?? '/', `http://${HOST}:${PORT}`).pathname);
  } catch {
    return null;
  }

  const relative = pathname === '/' ? 'index.html' : pathname.replace(/^\/+/, '');
  const candidate = resolve(ROOT, relative);
  const rootPrefix = ROOT.endsWith(sep) ? ROOT : `${ROOT}${sep}`;
  if (candidate !== resolve(ROOT, 'index.html') && !candidate.startsWith(rootPrefix)) return null;
  return candidate;
}

function writeHeaders(response, status, contentType = 'text/plain; charset=utf-8', extraHeaders = {}) {
  response.writeHead(status, {
    'Content-Type':contentType,
    'Cache-Control':'no-store',
    'X-Content-Type-Options':'nosniff',
    'Referrer-Policy':'no-referrer',
    'Permissions-Policy':'camera=(), geolocation=(), microphone=(self)',
    'Content-Security-Policy':"default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; media-src 'self' blob:; connect-src 'self'; object-src 'none'; base-uri 'none'; frame-ancestors 'none'",
    ...extraHeaders
  });
}

const server = createServer(async (request, response) => {
  if (!['GET','HEAD'].includes(request.method ?? '')) {
    writeHeaders(response,405,'text/plain; charset=utf-8',{ Allow:'GET, HEAD' });
    response.end('Method Not Allowed');
    return;
  }

  const filePath = safeFilePath(request.url);
  if (!filePath) {
    writeHeaders(response,400);
    response.end('Bad Request');
    return;
  }

  try {
    const info = await stat(filePath);
    if (!info.isFile()) throw new Error('not a file');
    const contentType = MIME[extname(filePath).toLowerCase()] ?? 'application/octet-stream';
    writeHeaders(response,200,contentType);
    if (request.method === 'HEAD') {
      response.end();
      return;
    }
    response.end(await readFile(filePath));
  } catch {
    writeHeaders(response,404);
    response.end('Not Found');
  }
});

server.listen(PORT,HOST,() => {
  console.log(`Falcon Shared Web browser verification server: http://${HOST}:${PORT}/`);
  console.log('Local verification only. No provider connectivity or deployment authority is created.');
  console.log('Press Ctrl+C to stop.');
});
