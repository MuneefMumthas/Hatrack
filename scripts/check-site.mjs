import fs from 'node:fs';
import path from 'node:path';
const root=path.resolve('dist');
const base=(process.env.BASE_PATH||'/hatrack').replace(/\/$/,'');
const files=[];function walk(dir){for(const e of fs.readdirSync(dir,{withFileTypes:true})){const f=path.join(dir,e.name);e.isDirectory()?walk(f):files.push(f)}}walk(root);
let failures=[];
for(const file of files.filter(f=>f.endsWith('.html'))){const html=fs.readFileSync(file,'utf8');if(!html.includes('rel="canonical"')||!html.includes('name="description"'))failures.push(`${file}: metadata missing`);for(const m of html.matchAll(/(?:href|src)="([^"#]+)"/g)){let url=m[1];if(!url.startsWith('/')||url.startsWith('//'))continue;if(base&&url.startsWith(base+'/'))url=url.slice(base.length);const resolved=path.join(root,decodeURIComponent(url.split('?')[0]));if(!fs.existsSync(resolved)&&!fs.existsSync(path.join(resolved,'index.html')))failures.push(`${file}: missing ${url}`);}}
if(failures.length){console.error(failures.join('\n'));process.exit(1)}console.log(`${files.filter(f=>f.endsWith('.html')).length} pages: metadata and local links passed.`);
