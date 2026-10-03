import {chromium} from '@playwright/test';
import fs from 'node:fs';
import path from 'node:path';
const browser=await chromium.launch({headless:true});
const out=path.resolve('../artifacts/visual');fs.mkdirSync(out,{recursive:true});
const base='http://127.0.0.1:4321/hatrack';
const errors=[];const results=[];
for(const viewport of [{width:1440,height:1000},{width:390,height:844}]){
 const page=await browser.newPage({viewport,deviceScaleFactor:1});page.on('pageerror',e=>errors.push(e.message));
 await page.goto(base+'/',{waitUntil:'networkidle'});await page.evaluate(()=>document.fonts.ready);
 const overflow=await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth);if(overflow)errors.push('Overflow '+viewport.width);
 const broken=await page.locator('img').evaluateAll(imgs=>imgs.filter(i=>!i.complete||i.naturalWidth===0).map(i=>i.src));errors.push(...broken.map(i=>'Broken image '+i));
 await page.screenshot({path:path.join(out,viewport.width===1440?'website-desktop.png':'website-mobile.png'),fullPage:true});
 results.push({viewport,overflow,brokenImages:broken.length,title:await page.title()});
 await page.getByRole('link',{name:'Get Hatrack',exact:true}).first().click();await page.waitForURL('**/download/');
 if(!await page.getByRole('heading',{name:'Download Hatrack for Windows'}).isVisible())errors.push('Download navigation failed');
 await page.goto(base+'/guides/',{waitUntil:'networkidle'});if(await page.locator('.guide-list>a').count()!==6)errors.push('Guide coverage incomplete');
 await page.close();
}
const social=await browser.newPage({viewport:{width:1200,height:630}});await social.goto('file:///'+path.resolve('public/social.svg').replaceAll('\\','/'));await social.screenshot({path:path.resolve('public/social.png')});await social.close();
await browser.close();fs.writeFileSync(path.join(out,'web-checks.json'),JSON.stringify({results,errors},null,2));console.log(JSON.stringify({results,errors},null,2));if(errors.length)process.exit(1);
