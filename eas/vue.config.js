/* jslint esversion: 6  */

module.exports = {
  chainWebpack: config => {
    config.module.rule('docs')
      // bundle common document files
      .test(/\.(pdf|docx?|xlsx?|csv|pptx?)(\?.*)?$/)
      .use('file-loader')
      // use the file-loader
      .loader('file-loader')
      // bundle into the "docs" directory
      .options({ name: 'docs/[name].[hash:8].[ext]' })
  },

  devServer: {
    disableHostCheck: true,
    proxy: {
      '/api': {
        target: 'http://localhost:54127',
        // ws: true,
        changeOrigin: true
      }
    },

//    host: '0.0.0.0',
//    port: '8080',
//    public: '192.168.254.104:8080',
//    disableHostCheck: true,
    hot: true
  },

  // pluginOptions: {
  //   webpackBundleAnalyzer: {
  //     openAnalyzer: false
  //   }
  // },

  configureWebpack : {

    optimization: {
      splitChunks: {
        chunks: 'all',
        // minSize: 30000,
        minSize: 0,
        maxSize: 0,
        minChunks: 1,
        maxAsyncRequests: 5,
        maxInitialRequests: 3,
        automaticNameDelimiter: '~',
        name: true,
        cacheGroups: {
          vendors: {
            test: /[\\/]node_modules[\\/]/,
            priority: -10,
            // name(module) {
            //   // get the name. E.g. node_modules/packageName/not/this/part.js
            //   // or node_modules/packageName
            //   const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];

            //   // npm package names are URL-safe, but some servers don't like @ symbols
            //   return `npm.${packageName.replace('@', '')}`;
            // }
          },
          // controls: {
          //   test: /[\\/src\\/]comp[\\/]/,
          //   priority: -5
          // },
          default: {
            minChunks: 2,
            priority: -20,
            reuseExistingChunk: true
          }
        }
      }
    },

    performance: {
      maxEntrypointSize: 250000,
      maxAssetSize: 250000
    }
  }
};