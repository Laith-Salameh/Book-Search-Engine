
$(document).ready(function () {
   
        $.widget("custom.bookcomplete", $.ui.autocomplete, {
            _create: function () {
                this._super();
                this.widget().menu("option", "items", "> :not(.ui-autocomplete-category)");
            },
            _renderMenu: function (ul, items) {
                var that = this,
                    currentCategory = "";
                $.each(items, function (index, item) {
                    var li;
                    if (item.category != currentCategory) {
                        ul.append("<li class='ui-autocomplete-category'>" + item.category + "</li>");
                        currentCategory = item.category;
                    }
                    li = that._renderItemData(ul, item);
                    if (item.category) {
                        li.attr("aria-label", item.category + " : " + item.label);
                    }
                });

            }
        });


    $('#searchText').bookcomplete({
        delay: 3,
                source: "book/search"
        });
    $(document).on("keyup", '#searchText',function () {
        var searchText = $(this).val();
        $.ajax({
            type: "Get",
            url: "book/getbooks",
            data: { searchValue: searchText },
            success: function (response) {
                var data = JSON.parse(response);
                console.log(data);
                if (data.length == 0) {
                    var html_body = `<div class="col-md-4 col-sm-6 col-xs-12 zoom">
                                                    <div class="probootstrap-room">
                                                        <div class="text">
                                                            <h4>No Results Found</h4>
                                                        </div>
                                                    </div>
                                                </div>`;
                    $('#room-cards').html(html_body);
                }
                else {
                    $('#room-cards').remove();
                    var html_body = `<div class="row" id="room-cards">

                                                </div>`;
                    $('.room-cards-container').append(html_body);
                    $.each(data, function (key, item) {
                        var html_search = `<div class="col-md-4 col-sm-6 col-xs-12 zoom">
                                                    <div class="probootstrap-room">
                                                        <img src="/img/book.jpg" class="probootstrap-room-img img-responsive ">
                                                        <div class="text">
                                                            <h4>${item.title}</h4>
                                                            <p>Auther Name: <strong>${item.author}</strong></p>
                                                            <p>Score: <strong>${item.score}</strong></p>
                                                        </div>
                                                    </div>
                                                </div>`;

                        $('#room-cards').append(html_search);

                    });
                    var books = $('.text');
                    var maxHeight = Math.max.apply(null, books.map(function () {
                        return $(this).height();
                    }).get());
                    books.height(maxHeight);
                }
                
            }
        })
    });

    $(document).on("click", ".ui-menu-item", function () {
        $('#searchText').trigger('keyup');
    });

});