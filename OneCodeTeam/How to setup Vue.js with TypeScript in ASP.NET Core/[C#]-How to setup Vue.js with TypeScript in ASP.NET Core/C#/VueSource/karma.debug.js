var webpackConfig = require('./config/webpack.config.test');

module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['source-map-support', 'mocha', 'chai', 'sinon'],
    files: [
      'node_modules/es6-promise/dist/es6-promise.auto.js',
      {
        pattern: "node_modules/es6-promise/dist/es6-promise.auto.map",
        watched: false,
        included: false,
        served: true
      },
      'src/test.ts'
    ],
    reporters: ['mocha'],
    preprocessors: {
      'src/test.ts': ['webpack']
    },
    webpack: webpackConfig,
    webpackServer: {
      noInfo: true
    },
    mime: {
      'text/x-typescript': ['ts']
    },
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: true,
    browsers: ["Chrome_with_debugging"],
    customLaunchers: {
      Chrome_with_debugging: {
        base: "Chrome",
        flags: ["--remote-debugging-port=9222"],
        debug: true
      }
    },
    singleRun: false
  });
};