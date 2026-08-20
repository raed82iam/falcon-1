import http from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import { extname, join, normalize } from 'node:path';
import { fileURLToPath } from 'node:url';

const root=fileURLToPath(new URL('../',import.meta.url));
const port=Number(process.env.FALCON_DEMO_PORT||4173);
const ownerEmail=String(process.env.FALCON_OWNER_EMAIL||'').trim().toLowerCase();
if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(ownerEmail)) {
  console.error('Set FALCON_OWNER_EMAIL to the Owner recovery email before starting the safe demo.');
  process.exit(1);
}
const mime={'.html':'text/html; charset=utf-8','.js':'text/javascript; charset=utf-8','.css':'text/css; charset=utf-8','.json':'application/json; charset=utf-8','.svg':'image/svg+xml','.png':'image/png','.webp':'image/webp'};
const headers={'Cache-Control':'no-store','X-Content-Type-Options':'nosniff','Referrer-Policy':'no-referrer','X-Frame-Options':'DENY','Content-Security-Policy':"default-src 'self'; img-src 'self' data:; style-src 'self'; script-src 'self'; connect-src 'self'; media-src 'self' blob:; object-src 'none'; base-uri 'self'; frame-ancestors 'none'"};
function send(res,status,body,type='text/plain; charset=utf-8'){res.writeHead(status,{...headers,'Content-Type':type});res.end(body);}
const server=http.createServer(async(req,res)=>{
  try{
    const url=new URL(req.url,'http://localhost');
    if(url.pathname==='/health') return send(res,200,JSON.stringify({status:'SAFE_DEMO_READY',foundation:'SIMULATED_NOT_ACTIVATED',fsats:'SIMULATED_NOT_CONNECTED',web:'READY',trading:false,broker:false,recovery:'EMAIL_ONLY'}),'application/json; charset=utf-8');
    if(url.pathname==='/local-demo-config.js'){
      const config={environment:'LOCAL_SAFE_DEMO',ownerEmail,businessAuthorityGranted:false,tradingAuthorityGranted:false,brokerAuthorityGranted:false};
      return send(res,200,`globalThis.__FALCON_WEB_RUNTIME_BINDINGS__=${JSON.stringify({mode:'PREVIEW',localSafeOwner:config})};\n`,'text/javascript; charset=utf-8');
    }
    const relative=url.pathname==='/'?'index.html':decodeURIComponent(url.pathname.slice(1));
    const candidate=normalize(join(root,relative));
    if(!candidate.startsWith(normalize(root))) return send(res,403,'Forbidden');
    const info=await stat(candidate);
    if(!info.isFile()) return send(res,404,'Not found');
    send(res,200,await readFile(candidate),mime[extname(candidate)]||'application/octet-stream');
  }catch{send(res,404,'Not found');}
});
server.listen(port,'127.0.0.1',()=>{
  console.log(`Falcon safe demo: http://127.0.0.1:${port}`);
  console.log(`Owner recovery: ${ownerEmail}`);
  console.log('Trading, broker connectivity, deployment, and external egress are disabled.');
});
