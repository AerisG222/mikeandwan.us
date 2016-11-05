/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};

/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {

/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;

/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};

/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);

/******/ 		// Flag the module as loaded
/******/ 		module.l = true;

/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}


/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;

/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;

/******/ 	// identity function for calling harmory imports with the correct context
/******/ 	__webpack_require__.i = function(value) { return value; };

/******/ 	// define getter function for harmory exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		Object.defineProperty(exports, name, {
/******/ 			configurable: false,
/******/ 			enumerable: true,
/******/ 			get: getter
/******/ 		});
/******/ 	};

/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};

/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };

/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";

/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 8);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ function(module, exports, __webpack_require__) {

"use strict";
"use strict";
var data_service_1 = __webpack_require__(4);
var background_1 = __webpack_require__(1);
var category_list_view_1 = __webpack_require__(2);
var nav_1 = __webpack_require__(5);
var Photos3D = (function () {
    function Photos3D() {
        this.clock = new THREE.Clock();
        this.dataService = new data_service_1.DataService();
        this.scene = new THREE.Scene();
        this.isPaused = false;
    }
    Photos3D.prototype.run = function () {
        var _this = this;
        // ensure scrollbars do not appear
        document.getElementsByTagName('body')[0].style.overflow = "hidden";
        window.addEventListener("resize", function () { return _this.onResize(); }, false);
        this.onResize();
        this.prepareScene();
        this.animate();
    };
    Photos3D.prototype.togglePause = function () {
        this.isPaused = !this.isPaused;
        this.animate();
    };
    Photos3D.prototype.toggleBackground = function () {
        if (this.background.isShown) {
            this.background.hide();
        }
        else {
            this.background.show();
        }
    };
    Photos3D.prototype.toggleAxisHelper = function () {
        if (this.axisHelper == null) {
            this.axisHelper = new THREE.AxisHelper(500);
            this.scene.add(this.axisHelper);
        }
        else {
            this.scene.remove(this.axisHelper);
            this.axisHelper = null;
        }
    };
    Photos3D.prototype.onResize = function () {
        this.width = window.innerWidth;
        this.height = window.innerHeight;
        if (this.width < 2200) {
            this.sizeCode = 'md';
        }
        else {
            this.sizeCode = 'lg';
        }
        // adjust view
        if (this.renderer != null) {
            this.renderer.setSize(this.width, this.height);
            this.camera.aspect = this.width / this.height;
        }
    };
    Photos3D.prototype.prepareScene = function () {
        // renderer
        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(this.width, this.height);
        document.body.appendChild(this.renderer.domElement);
        // camera
        this.camera = new THREE.PerspectiveCamera(45, this.width / this.height, 0.1, 2000);
        this.camera.position.set(0, 200, 1000);
        this.camera.lookAt(new THREE.Vector3(0, 200, 0));
        // ambient light
        this.ambientLight = new THREE.AmbientLight(0x404040);
        this.scene.add(this.ambientLight);
        // directional light
        this.directionalLight = new THREE.DirectionalLight(0xffffff, 0.9);
        this.directionalLight.position.set(-1, 1, 1);
        this.directionalLight.castShadow = true;
        this.scene.add(this.directionalLight);
        // background
        this.background = new background_1.Background(this.renderer, this.scene, this.camera, this.directionalLight, this.sizeCode);
        this.background.init();
        this.categoryListView = new category_list_view_1.CategoryListView(this.scene, this.width, this.height, this.dataService);
        this.categoryListView.init();
        this.toggleAxisHelper();
        this.nav = new nav_1.Nav();
        this.nav.init();
        this.animate();
    };
    Photos3D.prototype.animate = function () {
        var _this = this;
        if (this.isPaused) {
            return;
        }
        requestAnimationFrame(function () { return _this.animate(); });
        this.render();
        this.renderer.render(this.scene, this.camera);
    };
    Photos3D.prototype.render = function () {
        var delta = this.clock.getDelta();
        this.background.render(delta);
    };
    return Photos3D;
}());
exports.Photos3D = Photos3D;


/***/ },
/* 1 */
/***/ function(module, exports, __webpack_require__) {

"use strict";
"use strict";
var texture_loader_1 = __webpack_require__(7);
var Background = (function () {
    function Background(renderer, scene, camera, directionalLight, size) {
        this.renderer = renderer;
        this.scene = scene;
        this.camera = camera;
        this.directionalLight = directionalLight;
        this.size = size;
        this.horizontalRepeat = 2;
        this.waterUniform = null;
        this.textureLoader = new texture_loader_1.TextureLoader();
        this.isShown = true;
        if (renderer == null) {
            throw new Error('renderer should not be null');
        }
        if (scene == null) {
            throw new Error('scene should not be null');
        }
        if (camera == null) {
            throw new Error('camera should not be null');
        }
        if (size == null) {
            throw new Error('size should not be null');
        }
    }
    Background.prototype.init = function () {
        var texturePromise = this.loadTextures();
        this.updateTextures(texturePromise);
    };
    Background.prototype.setSize = function (size) {
        if (size == this.size) {
            return;
        }
        this.size = size;
        this.updateTextures(this.loadTextures());
    };
    Background.prototype.show = function () {
        if (this.isShown) {
            return;
        }
        if (this.treeMesh != null) {
            this.scene.add(this.treeMesh);
        }
        if (this.waterMesh != null) {
            this.scene.add(this.waterMesh);
        }
        this.isShown = true;
    };
    Background.prototype.hide = function () {
        if (!this.isShown) {
            return;
        }
        if (this.treeMesh != null) {
            this.scene.remove(this.treeMesh);
        }
        if (this.waterMesh != null) {
            this.scene.remove(this.waterMesh);
        }
        this.isShown = false;
    };
    Background.prototype.render = function (clockDelta) {
        if (this.isShown && this.waterUniform != null) {
            this.waterUniform.time.value += clockDelta;
        }
    };
    Background.prototype.loadTextures = function () {
        return this.textureLoader.loadTextures([
            ("/img/photos3d/" + this.size + "/" + Background.TEXTURE_NOISE),
            ("/img/photos3d/" + this.size + "/" + Background.TEXTURE_TREES),
            ("/img/photos3d/" + this.size + "/" + Background.TEXTURE_WATER)
        ]);
    };
    Background.prototype.updateTextures = function (texturePromises) {
        var _this = this;
        Promise.all(texturePromises).then(function (textures) {
            var waterTexture = null;
            var noiseTexture = null;
            for (var _i = 0, textures_1 = textures; _i < textures_1.length; _i++) {
                var texture = textures_1[_i];
                if (texture.name.indexOf(Background.TEXTURE_NOISE) > 0) {
                    noiseTexture = texture;
                }
                else if (texture.name.indexOf(Background.TEXTURE_TREES) > 0) {
                    _this.updateTrees(texture);
                }
                else if (texture.name.indexOf(Background.TEXTURE_WATER) > 0) {
                    waterTexture = texture;
                }
            }
            _this.updateWater(waterTexture, noiseTexture);
        }).catch(function (error) {
            console.error("error getting textures: " + error);
        });
    };
    Background.prototype.updateTrees = function (texture) {
        if (this.treeMesh != null) {
            this.scene.remove(this.treeMesh);
        }
        var treeGeometry = new THREE.PlaneGeometry(texture.image.width * this.horizontalRepeat, texture.image.height);
        this.treeMesh = new THREE.Mesh(treeGeometry);
        this.treeMesh.position.y = texture.image.height / 2;
        texture.repeat.set(this.horizontalRepeat, 1);
        texture.wrapS = THREE.MirroredRepeatWrapping;
        this.treeMesh.material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
        this.scene.add(this.treeMesh);
    };
    Background.prototype.updateWater = function (waterTexture, noiseTexture) {
        if (this.waterMesh != null) {
            this.scene.remove(this.waterMesh);
        }
        var waterGeometry = new THREE.PlaneGeometry(waterTexture.image.width * this.horizontalRepeat, waterTexture.image.height);
        this.waterMesh = new THREE.Mesh(waterGeometry);
        this.waterMesh.scale.y = -1;
        this.waterMesh.rotation.x = (Math.PI / 2);
        this.waterMesh.position.z = waterTexture.image.height / 2;
        waterTexture.repeat.set(this.horizontalRepeat, 1);
        waterTexture.wrapT = THREE.MirroredRepeatWrapping;
        waterTexture.wrapS = THREE.MirroredRepeatWrapping;
        noiseTexture.repeat.set(2, 1);
        noiseTexture.wrapT = THREE.MirroredRepeatWrapping;
        noiseTexture.wrapS = THREE.MirroredRepeatWrapping;
        this.waterUniform = {
            baseTexture: { type: "t", value: waterTexture },
            baseSpeed: { type: "f", value: 0.005 },
            noiseTexture: { type: "t", value: noiseTexture },
            noiseScale: { type: "f", value: 0.1 },
            alpha: { type: "f", value: 0.8 },
            time: { type: "f", value: 1.0 }
        };
        var waterMaterial = new THREE.ShaderMaterial({
            uniforms: this.waterUniform,
            vertexShader: document.getElementById('waterVertexShader').textContent,
            fragmentShader: document.getElementById('waterFragmentShader').textContent
        });
        waterMaterial.side = THREE.DoubleSide;
        this.waterMesh.material = waterMaterial;
        this.scene.add(this.waterMesh);
    };
    Background.TEXTURE_WATER = 'water.jpg';
    Background.TEXTURE_TREES = 'trees_blur20.jpg';
    Background.TEXTURE_NOISE = 'noise.png';
    return Background;
}());
exports.Background = Background;


/***/ },
/* 2 */
/***/ function(module, exports, __webpack_require__) {

"use strict";
"use strict";
var category_object3d_1 = __webpack_require__(3);
var linq_1 = __webpack_require__(6);
var CategoryListView = (function () {
    function CategoryListView(scene, width, height, dataService) {
        this.scene = scene;
        this.width = width;
        this.height = height;
        this.dataService = dataService;
        this.years = null;
    }
    CategoryListView.prototype.init = function () {
        this.loadCategories();
    };
    CategoryListView.prototype.show = function () {
    };
    CategoryListView.prototype.hide = function () {
    };
    CategoryListView.prototype.loadCategories = function () {
        var _this = this;
        this.dataService
            .getCategories()
            .then(function (categories) {
            var list = new linq_1.List(categories);
            var years = list.GroupBy(function (cat) { return cat.year; }, function (cat) { return cat; });
            for (var year in years) {
                if (years.hasOwnProperty(year)) {
                    _this.prepareCategoriesForYear(parseInt(year), years[year]);
                }
            }
            /*
            for(let i = 0; i < years.length; i++) {
                
                let category = categories[i];
                let year = this.getYear(category.year);
                let categoryObject = this.createCategoryObject(category);
                
                this.scene.add(categoryObject);
            }
            */
        });
    };
    CategoryListView.prototype.prepareCategoriesForYear = function (year, categories) {
        console.log(year);
    };
    CategoryListView.prototype.removeCategories = function () {
        if (this.years != null) {
            for (var i = 0; i < this.years.length; i++) {
                var year = this.years[i];
                for (var j = 0; j < year.categories.length; j++) {
                    this.scene.remove(year.categories[j]);
                }
            }
        }
    };
    CategoryListView.prototype.createCategoryObject = function (category) {
        var x = Math.random() * this.width - (this.width * 0.5);
        var y = Math.random() * this.height;
        var z = Math.random() * 200;
        var endPos = new THREE.Vector3(x, y, z);
        var color = Math.floor(Math.random() * 0xffffff);
        var cat = new category_object3d_1.CategoryObject3D(category, endPos, color);
        cat.position.x = x;
        cat.position.y = y;
        cat.position.z = z;
        cat.init();
        return cat;
    };
    CategoryListView.prototype.getYear = function (year) {
        var list = this.years.filter(function (iyear) { iyear.year == year; });
        if (list.length === 1) {
            return list[0];
        }
        else if (list.length === 0) {
            var y = { year: year, categories: [] };
            this.years.push(y);
            return y;
        }
        throw new Error('More than one entry found for the same year!');
    };
    return CategoryListView;
}());
exports.CategoryListView = CategoryListView;


/***/ },
/* 3 */
/***/ function(module, exports) {

"use strict";
"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var CategoryObject3D = (function (_super) {
    __extends(CategoryObject3D, _super);
    function CategoryObject3D(category, endPosition, color) {
        _super.call(this);
        this.color = color;
        this.mesh = null;
    }
    CategoryObject3D.prototype.init = function () {
        var geometry = new THREE.BoxGeometry(64, 64, 1);
        var material = new THREE.MeshLambertMaterial({
            color: this.color
        });
        this.mesh = new THREE.Mesh(geometry, material);
        this.add(this.mesh);
    };
    return CategoryObject3D;
}(THREE.Object3D));
exports.CategoryObject3D = CategoryObject3D;


/***/ },
/* 4 */
/***/ function(module, exports) {

"use strict";
"use strict";
var DataService = (function () {
    function DataService() {
        this.reqOpts = { method: 'get', credentials: 'include' };
    }
    DataService.prototype.getCategories = function () {
        return window.fetch('/api/photos/getAllCategories3D', this.reqOpts)
            .then(this.status)
            .then(this.json)
            .then(function (data) { return data; })
            .catch(function (err) {
            alert("There was an error getting data: {err}");
        });
    };
    DataService.prototype.getPhotos = function (categoryId) {
        return window.fetch("/api/photos/getPhotos3D/{categoryId}", this.reqOpts)
            .then(this.status)
            .then(this.json)
            .then(function (data) { return data; })
            .catch(function (err) {
            alert("There was an error getting data: {err}");
        });
    };
    // https://developers.google.com/web/updates/2015/03/introduction-to-fetch
    DataService.prototype.status = function (response) {
        if (response.status >= 200 && response.status < 300) {
            return Promise.resolve(response);
        }
        else {
            return Promise.reject(new Error(response.statusText));
        }
    };
    DataService.prototype.json = function (response) {
        return response.json();
    };
    return DataService;
}());
exports.DataService = DataService;


/***/ },
/* 5 */
/***/ function(module, exports) {

"use strict";
"use strict";
var Nav = (function () {
    function Nav() {
    }
    Nav.prototype.init = function () {
        this.navDiv = document.createElement('div');
        this.navDiv.id = 'nav';
        this.navDiv.style.position = 'absolute';
        this.navDiv.style.bottom = '0';
        this.navDiv.style.opacity = '0.7';
        this.navDiv.style.backgroundColor = '#333';
        this.navDiv.style.height = '24px';
        this.navDiv.style.width = '100%';
        document.body.appendChild(this.navDiv);
    };
    return Nav;
}());
exports.Nav = Nav;


/***/ },
/* 6 */
/***/ function(module, exports) {

"use strict";
"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
/**
 * LinQ to TypeScript
 *
 * Documentation from LinQ .NET specification (https://msdn.microsoft.com/en-us/library/system.linq.enumerable.aspx)
 *
 * Created by Flavio Corpa (@kutyel)
 * Copyright © 2016 Flavio Corpa. All rights reserved.
 *
 */
var List = (function () {
    /**
     * Defaults the elements of the list
     */
    function List(elements) {
        if (elements === void 0) { elements = []; }
        this._elements = elements;
    }
    /**
     * Adds an object to the end of the List<T>.
     */
    List.prototype.Add = function (element) {
        this._elements.push(element);
    };
    /**
     * Adds the elements of the specified collection to the end of the List<T>.
     */
    List.prototype.AddRange = function (elements) {
        (_a = this._elements).push.apply(_a, elements);
        var _a;
    };
    /**
     * Applies an accumulator function over a sequence.
     */
    List.prototype.Aggregate = function (accumulator, initialValue) {
        return this._elements.reduce(accumulator, initialValue);
    };
    /**
     * Determines whether all elements of a sequence satisfy a condition.
     */
    List.prototype.All = function (predicate) {
        return this._elements.every(predicate);
    };
    /**
     * Determines whether a sequence contains any elements.
     */
    List.prototype.Any = function (predicate) {
        return this._elements.some(predicate);
    };
    List.prototype.Average = function (transform) {
        return this.Sum(transform) / this.Count(transform);
    };
    /**
     * Concatenates two sequences.
     */
    List.prototype.Concat = function (list) {
        return new List(this._elements.concat(list.ToArray()));
    };
    /**
     * Determines whether an element is in the List<T>.
     */
    List.prototype.Contains = function (element) {
        return this._elements.some(function (x) { return x === element; });
    };
    List.prototype.Count = function (predicate) {
        return predicate ? this.Where(predicate).Count() : this._elements.length;
    };
    /**
     * Returns the elements of the specified sequence or the type parameter's default value
     * in a singleton collection if the sequence is empty.
     */
    List.prototype.DefaultIfEmpty = function (defaultValue) {
        return this.Count() ? this : new List([defaultValue]);
    };
    /**
     * Returns distinct elements from a sequence by using the default equality comparer to compare values.
     */
    List.prototype.Distinct = function () {
        return this.Where(function (value, index, iter) { return iter.indexOf(value) === index; });
    };
    /**
     * Returns the element at a specified index in a sequence.
     */
    List.prototype.ElementAt = function (index) {
        return this._elements[index];
    };
    /**
     * Returns the element at a specified index in a sequence or a default value if the index is out of range.
     */
    List.prototype.ElementAtOrDefault = function (index) {
        return this.ElementAt(index) || undefined;
    };
    /**
     * Produces the set difference of two sequences by using the default equality comparer to compare values.
     */
    List.prototype.Except = function (source) {
        return this.Where(function (x) { return !source.Contains(x); });
    };
    List.prototype.First = function (predicate) {
        return predicate ? this.Where(predicate).First() : this._elements[0];
    };
    List.prototype.FirstOrDefault = function (predicate) {
        return this.Count() ? this.First(predicate) : undefined;
    };
    /**
     * Performs the specified action on each element of the List<T>.
     */
    List.prototype.ForEach = function (action) {
        return this._elements.forEach(action);
    };
    /**
     * Groups the elements of a sequence according to a specified key selector function.
     */
    List.prototype.GroupBy = function (grouper, mapper) {
        return this.Aggregate(function (ac, v) { return (ac[grouper(v)] ? ac[grouper(v)].push(mapper(v)) : ac[grouper(v)] = [mapper(v)], ac); }, {});
    };
    /**
     * Correlates the elements of two sequences based on equality of keys and groups the results.
     * The default equality comparer is used to compare keys.
     */
    List.prototype.GroupJoin = function (list, key1, key2, result) {
        return this.Select(function (x, y) { return result(x, list.Where(function (z) { return key1(x) === key2(z); })); });
    };
    /**
     * Returns the index of the first occurence of an element in the List.
     */
    List.prototype.IndexOf = function (element) {
        return this._elements.indexOf(element);
    };
    /**
     * Produces the set intersection of two sequences by using the default equality comparer to compare values.
     */
    List.prototype.Intersect = function (source) {
        return this.Where(function (x) { return source.Contains(x); });
    };
    /**
     * Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
     */
    List.prototype.Join = function (list, key1, key2, result) {
        return this.SelectMany(function (x) { return list.Where(function (y) { return key2(y) === key1(x); }).Select(function (z) { return result(x, z); }); });
    };
    List.prototype.Last = function (predicate) {
        return predicate ? this.Where(predicate).Last() : this._elements[this.Count() - 1];
    };
    List.prototype.LastOrDefault = function (predicate) {
        return this.Count() ? this.Last(predicate) : undefined;
    };
    /**
     * Returns the maximum value in a generic sequence.
     */
    List.prototype.Max = function () {
        return this.Aggregate(function (x, y) { return x > y ? x : y; });
    };
    /**
     * Returns the minimum value in a generic sequence.
     */
    List.prototype.Min = function () {
        return this.Aggregate(function (x, y) { return x < y ? x : y; });
    };
    /**
     * Sorts the elements of a sequence in ascending order according to a key.
     */
    List.prototype.OrderBy = function (keySelector) {
        return new OrderedList(this._elements, ComparerHelper.ComparerForKey(keySelector, false));
    };
    /**
     * Sorts the elements of a sequence in descending order according to a key.
     */
    List.prototype.OrderByDescending = function (keySelector) {
        return new OrderedList(this._elements, ComparerHelper.ComparerForKey(keySelector, true));
    };
    /**
     * Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
     */
    List.prototype.ThenBy = function (keySelector) {
        return this.OrderBy(keySelector);
    };
    /**
     * Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
     */
    List.prototype.ThenByDescending = function (keySelector) {
        return this.OrderByDescending(keySelector);
    };
    /**
     * Removes the first occurrence of a specific object from the List<T>.
     */
    List.prototype.Remove = function (element) {
        return this.IndexOf(element) !== -1 ? (this.RemoveAt(this.IndexOf(element)), true) : false;
    };
    /**
     * Removes all the elements that match the conditions defined by the specified predicate.
     */
    List.prototype.RemoveAll = function (predicate) {
        return this.Where(this._negate(predicate));
    };
    /**
     * Removes the element at the specified index of the List<T>.
     */
    List.prototype.RemoveAt = function (index) {
        this._elements.splice(index, 1);
    };
    /**
     * Reverses the order of the elements in the entire List<T>.
     */
    List.prototype.Reverse = function () {
        return new List(this._elements.reverse());
    };
    /**
     * Projects each element of a sequence into a new form.
     */
    List.prototype.Select = function (mapper) {
        return new List(this._elements.map(mapper));
    };
    /**
     * Projects each element of a sequence to a List<any> and flattens the resulting sequences into one sequence.
     */
    List.prototype.SelectMany = function (mapper) {
        var _this = this;
        return this.Aggregate(function (ac, v, i) { return (ac.AddRange(_this.Select(mapper).ElementAt(i).ToArray()), ac); }, new List());
    };
    /**
     * Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
     */
    List.prototype.SequenceEqual = function (list) {
        return !!this._elements.reduce(function (x, y, z) { return list._elements[z] === y ? x : undefined; });
    };
    /**
     * Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
     */
    List.prototype.Single = function () {
        if (this.Count() !== 1) {
            throw new TypeError('The collection does not contain exactly one element.');
        }
        else {
            return this.First();
        }
    };
    /**
     * Returns the only element of a sequence, or a default value if the sequence is empty;
     * this method throws an exception if there is more than one element in the sequence.
     */
    List.prototype.SingleOrDefault = function () {
        return this.Count() ? this.Single() : undefined;
    };
    /**
     * Bypasses a specified number of elements in a sequence and then returns the remaining elements.
     */
    List.prototype.Skip = function (amount) {
        return new List(this._elements.slice(Math.max(0, amount)));
    };
    /**
     * Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
     */
    List.prototype.SkipWhile = function (predicate) {
        var _this = this;
        return this.Skip(this.Aggregate(function (ac, val) { return predicate(_this.ElementAt(ac)) ? ++ac : ac; }, 0));
    };
    List.prototype.Sum = function (transform) {
        return transform ? this.Select(transform).Sum() : this.Aggregate(function (ac, v) { return ac += (+v); }, 0);
    };
    /**
     * Returns a specified number of contiguous elements from the start of a sequence.
     */
    List.prototype.Take = function (amount) {
        return new List(this._elements.slice(0, Math.max(0, amount)));
    };
    /**
     * Returns elements from a sequence as long as a specified condition is true.
     */
    List.prototype.TakeWhile = function (predicate) {
        var _this = this;
        return this.Take(this.Aggregate(function (ac, val) { return predicate(_this.ElementAt(ac)) ? ++ac : ac; }, 0));
    };
    /**
     * Copies the elements of the List<T> to a new array.
     */
    List.prototype.ToArray = function () {
        return this._elements;
    };
    /**
     * Creates a Dictionary<TKey, TValue> from a List<T> according to a specified key selector function.
     */
    List.prototype.ToDictionary = function (key, value) {
        var _this = this;
        return this.Aggregate(function (o, v, i) { return (o[_this.Select(key).ElementAt(i)] = value ? _this.Select(value).ElementAt(i) : v, o); }, {});
    };
    /**
     * Creates a List<T> from an Enumerable.List<T>.
     */
    List.prototype.ToList = function () {
        return this;
    };
    /**
     * Creates a Lookup<TKey, TElement> from an IEnumerable<T> according to specified key selector and element selector functions.
     */
    List.prototype.ToLookup = function (keySelector, elementSelector) {
        return this.GroupBy(keySelector, elementSelector);
    };
    /**
     * Produces the set union of two sequences by using the default equality comparer.
     */
    List.prototype.Union = function (list) {
        return this.Concat(list).Distinct();
    };
    /**
     * Filters a sequence of values based on a predicate.
     */
    List.prototype.Where = function (predicate) {
        return new List(this._elements.filter(predicate));
    };
    /**
     * Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.
     */
    List.prototype.Zip = function (list, result) {
        var _this = this;
        return list.Count() < this.Count() ? list.Select(function (x, y) { return result(_this.ElementAt(y), x); }) :
            this.Select(function (x, y) { return result(x, list.ElementAt(y)); });
    };
    /**
     * Creates a function that negates the result of the predicate
     */
    List.prototype._negate = function (predicate) {
        return function () {
            return !predicate.apply(this, arguments);
        };
    };
    return List;
}());
exports.List = List;
var ComparerHelper = (function () {
    function ComparerHelper() {
    }
    ComparerHelper.ComparerForKey = function (_keySelector, descending) {
        return function (a, b) {
            return ComparerHelper.Compare(a, b, _keySelector, descending);
        };
    };
    ComparerHelper.Compare = function (a, b, _keySelector, descending) {
        var sortKeyA = _keySelector(a);
        var sortKeyB = _keySelector(b);
        if (sortKeyA > sortKeyB) {
            if (!descending) {
                return 1;
            }
            else {
                return -1;
            }
        }
        else if (sortKeyA < sortKeyB) {
            if (!descending) {
                return -1;
            }
            else {
                return 1;
            }
        }
        else {
            return 0;
        }
    };
    ComparerHelper.ComposeComparers = function (previousComparer, currentComparer) {
        return function (a, b) {
            var resultOfPreviousComparer = previousComparer(a, b);
            if (!resultOfPreviousComparer) {
                return currentComparer(a, b);
            }
            else {
                return resultOfPreviousComparer;
            }
        };
    };
    return ComparerHelper;
}());
/**
 * Represents a sorted sequence. The methods of this class are implemented by using deferred execution.
 * The immediate return value is an object that stores all the information that is required to perform the action.
 * The query represented by this method is not executed until the object is enumerated either by
 * calling its ToDictionary, ToLookup, ToList or ToArray methods
 */
var OrderedList = (function (_super) {
    __extends(OrderedList, _super);
    function OrderedList(elements, _comparer) {
        _super.call(this, elements);
        this._comparer = _comparer;
        this._elements.sort(this._comparer);
    }
    /**
     * Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
     * @override
     */
    OrderedList.prototype.ThenBy = function (keySelector) {
        return new OrderedList(this._elements, ComparerHelper.ComposeComparers(this._comparer, ComparerHelper.ComparerForKey(keySelector, false)));
    };
    /**
     * Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
     * @override
     */
    OrderedList.prototype.ThenByDescending = function (keySelector) {
        return new OrderedList(this._elements, ComparerHelper.ComposeComparers(this._comparer, ComparerHelper.ComparerForKey(keySelector, true)));
    };
    return OrderedList;
}(List));
var Enumerable = (function () {
    function Enumerable() {
    }
    /**
     * Generates a sequence of integral numbers within a specified range.
     */
    Enumerable.Range = function (start, count) {
        var result = new List();
        while (count--) {
            result.Add(start++);
        }
        return result;
    };
    /**
     * Generates a sequence that contains one repeated value.
     */
    Enumerable.Repeat = function (element, count) {
        var result = new List();
        while (count--) {
            result.Add(element);
        }
        return result;
    };
    return Enumerable;
}());
exports.Enumerable = Enumerable;


/***/ },
/* 7 */
/***/ function(module, exports) {

"use strict";
"use strict";
var TextureLoader = (function () {
    function TextureLoader() {
        this.loader = new THREE.TextureLoader();
    }
    TextureLoader.prototype.loadTexture = function (url) {
        var _this = this;
        return new Promise(function (resolve, reject) {
            _this.loader.load(url, function (texture) {
                texture.name = url;
                resolve(texture);
            }, function (progress) {
                console.log("downloading " + url + ": " + progress.loaded);
            }, function (error) {
                reject(error);
            });
        });
    };
    TextureLoader.prototype.loadTextures = function (urls) {
        var list = [];
        for (var i = 0; i < urls.length; i++) {
            list.push(this.loadTexture(urls[i]));
        }
        return list;
    };
    return TextureLoader;
}());
exports.TextureLoader = TextureLoader;


/***/ },
/* 8 */
/***/ function(module, exports, __webpack_require__) {

"use strict";
"use strict";
var photos3d_1 = __webpack_require__(0);
var app = new photos3d_1.Photos3D();
app.run();
Mousetrap.bind("space", function (e) { app.togglePause(); });
Mousetrap.bind("b", function (e) { app.toggleBackground(); });
Mousetrap.bind("x", function (e) { app.toggleAxisHelper(); });


/***/ }
/******/ ]);