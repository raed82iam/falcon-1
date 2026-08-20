import test from 'node:test';
import assert from 'node:assert/strict';
import { createWebDataSource, DataSourceMode } from '../src/core/data-source-profile.js';

test('preview mode requires preview data and rejects authoritative coexistence',()=>{
  const preview={portfolio:{}};
  const source=createWebDataSource({mode:DataSourceMode.PREVIEW,previewData:preview});
  assert.equal(source.preview,true);
  assert.equal(source.authoritative,false);
  assert.equal(source.data,preview);
  assert.throws(()=>createWebDataSource({mode:DataSourceMode.PREVIEW,previewData:preview,authoritativeData:{}}),/must not coexist/);
});

test('authoritative mode rejects preview data',()=>{
  assert.throws(()=>createWebDataSource({mode:DataSourceMode.AUTHORITATIVE,previewData:{}}),/must not receive previewData/);
});

test('authoritative mode fails closed when authoritative data is unavailable',()=>{
  const source=createWebDataSource({mode:DataSourceMode.AUTHORITATIVE});
  assert.equal(source.authoritative,false);
  assert.equal(source.preview,false);
  assert.equal(source.unavailable,true);
  assert.deepEqual(source.data.positions,[]);
});

test('authoritative mode exposes only supplied authoritative data',()=>{
  const authoritative={portfolio:{totalEquity:100},positions:[]};
  const source=createWebDataSource({mode:DataSourceMode.AUTHORITATIVE,authoritativeData:authoritative});
  assert.equal(source.authoritative,true);
  assert.equal(source.preview,false);
  assert.equal(source.data,authoritative);
});
