// This is the "Offline page" service worker

//importScripts('https://storage.googleapis.com/workbox-cdn/releases/5.1.2/workbox-sw.js');

importScripts('/lib/third_party/workbox/workbox-v7.1.0/workbox-sw.js');

workbox.setConfig({
    modulePathPrefix: '/lib/third_party/workbox/workbox-v7.1.0/',
});

const CACHE = "pwabuilder-page";

// TODO: replace the following with the correct offline fallback page i.e.: const offlineFallbackPage = "offline.html";
const offlineFallbackPage = "no-internet.html";

self.addEventListener("message", (event) => {
  if (event.data && event.data.type === "SKIP_WAITING") {
    self.skipWaiting();
  }
});

self.addEventListener('install', async (event) => {
  event.waitUntil(
    caches.open(CACHE)
      .then((cache) => cache.add(offlineFallbackPage))
  );
});

if (workbox.navigationPreload.isSupported()) {
  workbox.navigationPreload.enable();
}

self.addEventListener('fetch', (event) => {
  if (event.request.mode === 'navigate') {
    event.respondWith((async () => {
      try {
        const preloadResp = await event.preloadResponse;

        if (preloadResp) {
          return preloadResp;
        }

        const networkResp = await fetch(event.request);
        return networkResp;
      } catch (error) {

        const cache = await caches.open(CACHE);
        const cachedResp = await cache.match(offlineFallbackPage);
        return cachedResp;
      }
    })());
  }
});

async function cacheFirst(request, cacheName) {
    const cachedResponse = await caches.match(request);
    if (cachedResponse) {
        return cachedResponse;
    }
    try {
        const networkResponse = await fetch(request);
        if (networkResponse.ok) {
            const cache = await caches.open(cacheName);
            cache.put(request, networkResponse.clone());
        }
        return networkResponse;
    } catch (error) {
        return Response.error();
    }
}

self.addEventListener("fetch", (event) => {
    const url = new URL(event.request.url);
    if (event.request.destination == "image") {
        if (url.pathname.startsWith('/project')) {
            event.respondWith(cacheFirst(event.request, "Project"));
        } else if (url.pathname.startsWith('/photo')) {
            event.respondWith(cacheFirst(event.request, "Photo"));
        } else if (url.pathname.startsWith('/picture')) {
            event.respondWith(cacheFirst(event.request, "Picture"));
        } else if (url.pathname.startsWith('/watermark')) {
            event.respondWith(cacheFirst(event.request, "Watermark"));
        }
    }

    //console.log(event.request.destination)
    //if (url.pathname.startsWith('/project') && (url.pathname.toLowerCase().endsWith('.jpg') || url.pathname.toLowerCase().endsWith('.jpeg') || url.pathname.toLowerCase().endsWith('.png'))) {
    //    event.respondWith(cacheFirst(event.request, "Project"));
    //}

    //if (url.pathname.startsWith('/photo') && (url.pathname.toLowerCase().endsWith('.jpg') || url.pathname.toLowerCase().endsWith('.jpeg') || url.pathname.toLowerCase().endsWith('.png'))) {
    //    event.respondWith(cacheFirst(event.request, "Photo"));
    //}
});