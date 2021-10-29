var app = angular.module('FoodOrderApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('FoodMenuController', ['$scope', '$http', function ($scope, $http) {
    $scope.refresh = function () {
        $http.get('api/Food')
            .then(function (response) {
                $scope.foodMenu = response.data;
                console.log("Response success");
                console.log(resposne);
            }, function (error) {
                $scope.foodMenu = undefined;
            });
    }

    $http.remove = function (name) {
        $http.delete('api/Food/' + name)
            .then(function (data, status) {
                $scope.refresh();
            })
    };

    $http.update = function (item) {
        $http.post('api/Food', item)
            .then(function (data, status) {
                $scope.refresh();
            });
    };

    $scope.addToMenu = function (item) {
        $http.update(item);
    }

    $scope.removeFromMenu = function (item) {
        $http.remove(item.productName);
    }
}]);

app.controller('OrderCartController', ['$scope', '$http', function ($scope, $http) {
    $scope.refresh = function () {
        $http.get('api/OrderCart')
            .then(function (response) {
                $scope.cart = response.data;
            }, function (error) {
                $scope.cart = undefined;
            });
    }

    $http.remove = function (name) {
        $http.delete('api/OrderCart/' + name)
            .then(function (data, status) {
                $scope.refresh();
            })
    };

    $http.update = function (item) {
        $http.post('api/OrderCart', item)
            .then(function (data, status) {
                $scope.refresh();
            });
    };

    $scope.addToCart = function (item) {
        $http.update(item);
    }

    $scope.removeFromCart = function (item) {
        $http.remove(item.productName);
    }
}]);
