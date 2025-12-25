import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import mkcert from "vite-plugin-mkcert";

export default defineConfig({
  plugins: [react(), mkcert()],
  server: {
    port: 5173,
    https: true as any,
    proxy: {
      '/api': {
        target: 'http://localhost:5145',
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
