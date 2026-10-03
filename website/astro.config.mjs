import { defineConfig } from 'astro/config';
export default defineConfig({ site:process.env.SITE_URL || 'https://muneefmumthas.github.io', base:process.env.BASE_PATH || '/hatrack', trailingSlash:'always', output:'static' });
