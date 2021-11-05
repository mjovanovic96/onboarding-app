var app = angular.module('FoodOrderApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('FoodMenuCartController', ['$scope', '$http', function ($scope, $http) {
    $scope.refresh = function () {
        $http.get('api/FoodMenu')
            .then((response) => {
                $scope.foodMenu = response.data;
            }, (error) => {
                $scope.foodMenu = undefined;
                console.log(error);
            });

        $http.get('/api/Carts')
            .then(
                (response) => {
                $scope.cart = response.data;
            },
                (error) => {
                    $scope.cart = undefined;
                    console.log(error);
            });
    }

    $http.removeItemFromMenu = function (name) {
        $http.delete('api/FoodMenu/' + name)
            .then(function (data, status) {
                window.location.reload();
            })
    };

    $http.updateMenu = function (item) {
        $http.post('api/FoodMenu', item)
            .then(function (data, status) {
                window.location.reload();
            });
    };

    $http.removeItemFromCart = function (name) {
        $http.delete('api/Carts/' + name)
            .then(function (data, status) {
                $window.location.reload();
            })
    };

    $http.updateCart = function (item) {
        $http.post('api/Carts', item)
            .then(function (data, status) {
                window.location.reload();
            });
    };

    $http.removeCart = function () {
        $http.delete('api/Carts/')
            .then(function (data, status) {
                window.location.reload();
            })
    };


    $scope.addToMenu = function (item) {
        $http.updateMenu(item);
    }

    $scope.removeFromMenu = function (item) {
        $http.removeItemFromMenu(item.name);
    }

    $scope.addToCart = function (item) {
        $http.updateCart(item);
    }

    $scope.removeFromCart = function (item) {
        $http.removeItemFromCart(item.name);
    }

    $scope.removeCart = function () {
        $http.removeCart();
    }
}]);