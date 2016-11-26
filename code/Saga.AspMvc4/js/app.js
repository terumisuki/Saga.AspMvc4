'use strict';


// Taken and adapted from
//      http://www.script-tutorials.com/demos/366/index.html

angular.module('slideshow', ['ngAnimate', 'ngTouch'])
  .controller('MainCtrl', function ($scope, $http, $timeout, $window, $location) {


      // This is a test.

      // initialize
      $scope.showImage = false;
      $scope.timer = 16000;
      $scope.usercode = "notfound";
      $scope.isPlaying = true;
      $scope.currentAction = "slideShow";
      

      $scope.login = function () {
          if ($scope.username == '' || $scope.password == '' || $scope.username == undefined || $scope.password == undefined) {
              return;
          }

          $http.get('/api/user?username=' + $scope.username + '&password=' + $scope.password)
            .success(function (data) {
                if (data == 'notfound') { return; }

                $scope.usercode = data;
                $scope.refresh();
                $scope.showImage = true;
            });
      }

      $scope.refresh = function () {
          if (!$scope.isPlaying) {
              return;
          }
          $scope.showImage = false;
          if ($scope.currentAction == "slideShow") {
              $scope.imageA = '/api/image/?code=' + $scope.usercode + '&width=' + $window.innerWidth + '&height=' + $window.innerHeight + '&timestamp=' + new Date().getTime();
          } else if ($scope.currentAction == "photoAlbum") {
              // get the info of next image to show
              $http.get('/api/photoalbumnextphoto/?acode=' + $scope.albumCode + '&auth=' + $scope.usercode + '&timestamp=' + new Date().getTime())
                .success(function (data) {
                    $scope.albumPhoto = data;
                    // get the image
                    $scope.imageA = '/api/photoalbum/?auth=' + $scope.usercode + '&photoid=' + $scope.albumPhoto.MediaId + '&width=' + $window.innerWidth + '&height=' + $window.innerHeight + '&timestamp=' + new Date().getTime();
                });
          }
          
          $timeout(function () { $scope.showImage = true; }, 2500);
          if ($scope.isPlaying) {
              $timeout(function () {
                  $scope.refresh();
              }, $scope.timer);
          }
      };

      $scope.toggleControls = function () {
          $scope.isPlaying = !$scope.isPlaying;
          if ($scope.isPlaying) { $scope.refresh(); }
      }





      // for playing a photo album from an emailed link
      if ($location.search().albumCode && $location.search().auth) {
          $scope.albumCode = $location.search().albumCode;
          $scope.usercode = $location.search().auth;
          $scope.currentAction = "photoAlbum";
          $scope.refresh();
          $scope.showImage = true;
      }


  });
