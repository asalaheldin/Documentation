$(document).ready(function () {
    $('body').on('click', '.order', function (e) {
        var $this = $(this);
        var field = $this.attr('data-field');
        var dir = $this.attr('data-dir');
        var pageIndex = 1;
        var pageSize = 10;
        var query = $('#query').val();
        var date = $('#filterdate').val();
        var type = $('#type').val();
        
        $.ajax({
            type: "get",
            url: "/document/index?isPartial=true&orderByField=" + field + "&orderDir=" + dir + "&query=" + query + "&date=" + date + "&type=" + type + "&page=" + pageIndex + "&pageSize=" + pageSize,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.status === 'success') {
                    $('#tableBody').html(response.partialView);
                    //Toggle order direction
                    $this.attr('data-dir', function (n, v) {
                        if (v === 'asc') { return 'desc'; }
                        else { return 'asc'; }
                    });
                }
                else {
                    console.log('error');
                }
            },
            failure: function (response) {
                console.log(response);
            }
        });
    })


    $('body').on('keyup', '#query, #filterdate', function (e) {
        if (e.keyCode == 13 || $(this).val() === '') {
            doSearch();
        }
    });
    $('body').on('change', '#type', function (e) {
        doSearch();
    });
    $('#filterdate').datepicker({
        onSelect: function (date) {
            alert(date)
        }
    });

    function doSearch() {
        var pageIndex = 1;
        var pageSize = 10;
        var query = $('#query').val();
        var date = $('#filterdate').val();
        var type = $('#type').val();

        $.ajax({
            type: "get",
            url: "/document/index?isPartial=true" + "&query=" + query + "&date=" + date + "&type=" + type + "&page=" + pageIndex + "&pageSize=" + pageSize,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.status === 'success') {
                    $('#tableBody').html(response.partialView);
                }
                else {
                    console.log('error');
                }
            },
            failure: function (response) {
                console.log(response);
            }
        });

    }
});