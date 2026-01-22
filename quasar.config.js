const { configure } = require('quasar/wrappers');

module.exports = configure(function (/* ctx */) {
  return {
    boot: [],
    css: ['app.scss'],
    extras: [
      'fontawesome-v6',
      'material-icons'
    ],
    build: {
      target: {
        browser: ['es2019', 'edge88', 'firefox78', 'chrome87', 'safari13.1'],
        node: 'node16'
      },
      vueRouterMode: 'hash'
    },
    devServer: {
      open: true
    },
    framework: {
      config: {
        notify: {}
      },
      plugins: [
        'Notify'
      ]
    },
    animations: [],
    ssr: {
      pwa: false
    },
    pwa: {
      workboxMode: 'generateSW',
      injectPwaMetaTags: true,
      swFilename: 'sw.js',
      manifest: {
        name: 'HVAC Asset Pro',
        short_name: 'HVAC Asset Pro',
        description: 'Professional HVAC management tool for Air Conditioning Business',
        display: 'standalone',
        orientation: 'portrait',
        background_color: '#ffffff',
        theme_color: '#027be3',
        icons: [
          {
            src: 'icons/icon-128x128.png',
            sizes: '128x128',
            type: 'image/png'
          }
        ]
      }
    }
  }
});

