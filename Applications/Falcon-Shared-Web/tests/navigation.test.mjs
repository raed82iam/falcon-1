import test from 'node:test';
import assert from 'node:assert/strict';
import { AppRoute, SurfaceKind, normalizeRoute, routeBelongsToSurface, routeHash } from '../src/platform/navigation/routes.js';

test('known routes normalize and unknown routes fail to public home', () => {
  assert.equal(normalizeRoute('#/portfolio'), AppRoute.PORTFOLIO);
  assert.equal(normalizeRoute('/owner-incidents'), AppRoute.OWNER_INCIDENTS);
  assert.equal(normalizeRoute('/owner-ai-emergency'), AppRoute.OWNER_AI_EMERGENCY);
  assert.equal(normalizeRoute('not-a-real-route'), AppRoute.HOME);
});

test('route metadata separates public, user and owner surfaces', () => {
  assert.equal(routeBelongsToSurface(AppRoute.HOME, SurfaceKind.PUBLIC), true);
  assert.equal(routeBelongsToSurface(AppRoute.TRADER_HOME, SurfaceKind.USER), true);
  assert.equal(routeBelongsToSurface(AppRoute.OWNER_HOME, SurfaceKind.OWNER), true);
  assert.equal(routeBelongsToSurface(AppRoute.OWNER_AI_EMERGENCY, SurfaceKind.OWNER), true);
  assert.equal(routeBelongsToSurface(AppRoute.OWNER_AI_EMERGENCY, SurfaceKind.USER), false);
});

test('route hash is generated only from the canonical registry', () => {
  assert.equal(routeHash(AppRoute.FSATS_PUBLIC), '#/fsats');
  assert.equal(routeHash(AppRoute.OWNER_AI_EMERGENCY), '#/owner-ai-emergency');
  assert.equal(routeHash('malformed'), '#/home');
});
