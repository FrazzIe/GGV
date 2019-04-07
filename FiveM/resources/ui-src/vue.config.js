module.exports = {
  baseUrl: './',
  outputDir: '../ggv/ui',
  productionSourceMap: false,
  configureWebpack: {
    optimization: {
      minimize: true,
    },
  },
  filenameHashing: false,
  chainWebpack: (config) => {
    config.optimization.splitChunks(false);
  },
}