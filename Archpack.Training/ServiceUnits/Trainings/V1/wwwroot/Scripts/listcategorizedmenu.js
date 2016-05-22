; (function () {

    App.ui.listmenu = {

        create: function (element, data, urlResolver) {
            var container = $(element)[0];
            var menuItems = [],
                menuCategories = [],
                emptyMenuCategoryItems;

            function procItems(items) {

                items.forEach(function (item) {
                    if (item.path && item.url && item.display) {
                        menuItems.push(item);
                        if (item.category) {
                            if (!menuCategories.some(function (c) {
                                return c.name === item.category;
                            })) {
                                menuCategories.push({
                                    name: item.category,
                                    display: item.category
                                });
                            }
                        }
                    }
                    if (item.menu && App.isArray(item.menu.categories)) {
                        item.menu.categories.forEach(function (defCategory) {
                            if (!menuCategories.some(function (c) {
                                var result = defCategory.name === c.name;
                                if (result && !c.index) {
                                    c.index = defCategory.index;
                            }
                                return result;
                            })) {
                                menuCategories.push(defCategory);
                            }
                        });
                    }
                    if (App.isArray(item.items)) {
                        procItems(item.items);
                    }
                });
            }

            procItems(data.items);

            menuCategories.sort(function (left, right) {
                if (!left.index) left.index = Number.MAX_VALUE;
                if (!right.index) right.index = Number.MAX_VALUE;
                return left.index < right.index ? -1 :
                       right.index > left.index ? 1 : 0;
            });
            menuCategories.forEach(function (category) {
                var items = menuItems.filter(function (item) {
                    return category.name === item.category;
                });
                items.sort(function (left, right) {
                    if (!left.index) left.index = Number.MAX_VALUE;
                    if (!right.index) right.index = Number.MAX_VALUE;
                    return left.index < right.index ? -1 :
                           right.index > left.index ? 1 : 0;
                });
                category.items = items;
            });

            //empty category items

            emptyMenuCategoryItems = menuItems.filter(function (item) {
                return !item.category;
            });
            emptyMenuCategoryItems.sort(function (left, right) {
                if (!left.index) left.index = Number.MAX_VALUE;
                if (!right.index) right.index = Number.MAX_VALUE;
                return left.index < right.index ? -1 :
                       right.index > left.index ? 1 : 0;
            });
            if (emptyMenuCategoryItems && emptyMenuCategoryItems.length) {
                menuCategories.push({
                    name: "",
                    display: "",
                    index: Number.MAX_VALUE,
                    items: emptyMenuCategoryItems
                });
            }

            var containerFrag = document.createDocumentFragment();
            menuCategories.forEach(function (category, index) {

                var categoryElem = document.createElement("div"),
                    checkElem = document.createElement("input"),
                    labelElem = document.createElement("label"),
                    listContainerElem = document.createElement("ul"),
                    toggleId = "list-menu-toggle-id" + index;

                checkElem.type = "checkbox";
                checkElem.id = toggleId;
                checkElem.checked = true;
                labelElem.htmlFor = toggleId

                listContainerElem.classList.add("list-menu-items");

                categoryElem.classList.add("list-menu-item-box");

                checkElem.classList.add("list-menu-item-box-toggle");
                labelElem.classList.add("list-menu-item-box-title");
                labelElem.textContent = category.display;
                categoryElem.appendChild(checkElem);
                categoryElem.appendChild(labelElem);

                category.items.forEach(function (item) {
                    var listItemElem = document.createElement("li"),
                        linkElem = document.createElement("a");
                    listItemElem.classList.add("list-menu-item");
                    linkElem.classList.add("list-menu-item-link");
                    linkElem.href = urlResolver((item.path + "/page/" + (item.url + "").split("?")[0]));
                    linkElem.textContent = item.display;
                    listItemElem.appendChild(linkElem);
                    listContainerElem.appendChild(listItemElem);
                });

                categoryElem.appendChild(listContainerElem);
                containerFrag.appendChild(categoryElem);
            });
            container.appendChild(containerFrag);
        }

    };

})();