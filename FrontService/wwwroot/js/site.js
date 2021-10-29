var app = angular.module('FoodOrderApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('FoodMenuCartController', ['$scope', '$http', function ($scope, $http) {
    $scope.refresh = function () {
        $http.get('api/FoodMenu')
            .then((response) => {
                $scope.foodMenu = response.data;
                console.log("Food Menu Response");
                console.log(response);
            }, (error) => {
                $scope.foodMenu = undefined;
                console.log(error);
            });

        $http.get('/api/Carts')
            .then((response) => {
                $scope.cart = response.data;
                console.log("Cart response");
                console.log(response);
            },
                (error) => {
                    $scope.cart = undefined;
                    console.log(error);
            });
    }

    $http.removeItemFromMenu = function (name) {
        $http.delete('api/FoodMenu/' + name)
            .then(function (data, status) {
                // $scope.refresh();
            })
    };

    $http.updateMenu = function (item) {
        $http.post('api/FoodMenu', item)
            .then(function (data, status) {
                // $scope.refresh();
            });
    };

    $http.removeItemFromCart = function (name) {
        $http.delete('api/Carts/' + name)
            .then(function (data, status) {
                // $scope.refresh();
            })
    };

    $http.updateCart = function (item) {
        $http.post('api/Carts', item)
            .then(function (data, status) {
                // $scope.refresh();
            });
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
}]);