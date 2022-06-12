$(function () {
    var placeholder = $("#modal-placeholder");
    $(document).on('click', 'button[data-toggle="ajax-modal"]', function () {
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () { ShowLoading(); },
            complete: function () { $("body").preloader('remove'); },
            error: function () {
                ShowSweetErrorAlert();
            }
        }).done(function (result) {
            placeholder.html(result);
            placeholder.find('.modal').modal('show');
        });
    });


    placeholder.on('click', 'button[data-save="modal"]', function () {
        ShowLoading();
        var form = $(this).parents(".modal").find('form');
        var actionUrl = form.attr('action');
        if (form.length == 0) {
            form = $(".card-body").find('form');
            actionUrl = form.attr('action') + '/' + $(".modal").attr('id');
            var result = actionUrl.includes('Index/DeleteGroup');
            if (result == true)
                actionUrl = actionUrl.replace("Index/", "");
        }
        var dataToSend = new FormData(form.get(0));

        $.ajax({
            url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, error: function () {
                ShowSweetErrorAlert();
            }
        }).done(function (data) {
            var newBody = $(".modal-body", data);
            var newFooter = $(".modal-footer", data);
            placeholder.find(".modal-body").replaceWith(newBody);
            placeholder.find(".modal-footer").replaceWith(newFooter);

            var IsValid = newBody.find("input[name='IsValid']").val() === "True";
            if (IsValid) {
                $.ajax({ url: '/Admin/Base/Notification', error: function () { ShowSweetErrorAlert(); } }).done(function (notification) {
                    ShowSweetSuccessAlert(notification)
                });

                $table.bootstrapTable('refresh')
                placeholder.find(".modal").modal('hide');
            }
        });

        $("body").preloader('remove');
    });

    $(document).on('click', 'a[data-toggle="ajax-modal"]', function () {
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () { $('#modal-placeholder').after('<div class="preloader d-flex align-items-center justify-content-center"><div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div></div>'); },
            complete: function () { $('.preloader').remove(); },
            error: function () {
                ShowSweetErrorAlert();
            }
        }).done(function (result) {
            placeholder.html(result);
            placeholder.find('.modal').modal('show');
        });
    });

    placeholder.on('click', 'button[data-save="modal-ajax"]', function () {
        var form;
        var IsValid;
        var btnId = $(this).attr('id');
        if (btnId == "btn-register") {
            form = $(this).parents(".modal").find('#pills-signout form');
        }
        else if (btnId == "btn-signin") {
            form = $(this).parents(".modal").find('#pills-signin form');
        }
        else {
            form = $(this).parents(".modal").find('form');
        }

        var actionUrl = form.attr('action');
        var dataToSend = new FormData(form.get(0));
        $.ajax({
            url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, error: function () {
                ShowSweetErrorAlert();
            },
            beforeSend: function () { $('#modal-placeholder').after('<div class="preloader d-flex align-items-center justify-content-center"><div class="lds-ellipsis"><div></div><div></div><div></div><div></div></div></div>'); },
        }).done(function (data) {
            if (btnId == "btn-register") {
                $('#pills-signout').html(data);
                IsValid = $('#pills-signout').find("input[name='IsValid']").val() === "True";
                if (IsValid) {
                    $.ajax({ url: '/Admin/Base/Notification', error: function () { ShowSweetErrorAlert(); } }).done(function (notification) {
                        ShowSweetSuccessAlert(notification)
                    });
                    placeholder.find(".modal").modal('hide');
                }
            }
            else if (btnId == "btn-signin") {
                if (data == "success") {
                    window.location.href = '/Account/Profile/';
                }
                else if (data == "requiresTwoFactor") {
                    alert("requires");
                }
                else {
                    $('#pills-signin').html($("#pills-signin", data));
                }
            }
            else {
                if (($(data).find(".modal-body").length) == 0) {
                    $("#bookmarks").html(data);
                    placeholder.find(".modal").modal('hide');
                }
                else {
                    var newBody = $(".modal-body", data);
                    var newFooter = $(".modal-footer", data);
                    placeholder.find(".modal-body").replaceWith(newBody);
                    placeholder.find(".modal-footer").replaceWith(newFooter);

                }
            }
            $('.preloader').remove();
        });
    });
});

function ShowLoading() {
    $("body").preloader({ text: 'لطفا صبر کنید ...' });
}

function ShowSweetErrorAlert() {
    Swal.fire({
        type: 'error',
        title: 'خطایی رخ داده است !!!',
        text: 'لطفا تا برطرف شدن خطا شکیبا باشید.',
        confirmButtonText: 'بستن'
    });
}

function ShowErrorMessage(message) {
    Swal.fire({
        position: 'top-middle',
        title: message,
        text: 'خطا !!!',
        confirmButtonText: 'بستن'
    });
}

function ShowSweetSuccessAlert(message) {
    Swal.fire({
        position: 'top-middle',
        type: 'success',
        title: message,
        confirmButtonText: 'بستن',
    })
}

function ShowSweetAlert(message) {
    Swal.fire({
        position: 'top-middle',
        title: message,
        confirmButtonText: 'بستن',
    })
}

$(document).on('click', 'button[data-save="Ajax"]', function () {
    var form = $(".newsletter-widget").find('form');
    var actionUrl = form.attr('action');
    var dataToSend = new FormData(form.get(0));

    $.ajax({
        url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false, error: function () {
            ShowSweetErrorAlert();
        }
    }).done(function (data) {
        var newForm = $("form", data);
        $(".newsletter-widget").find("form").replaceWith(newForm);
        var IsValid = newForm.find("input[name='IsValid']").val() === "True";
        if (IsValid) {
            $.ajax({ url: '/Admin/Base/Notification', error: function () { ShowSweetErrorAlert(); } }).done(function (notification) {
                ShowSweetSuccessAlert(notification)
            });
        }
    });
});

function ConfigureSettings(id, action) {
    $.ajax({
        url: "/Admin/UserManager/" + action + "?userId=" + id,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    }).done(function (result) {
        if (result == "فعال" || result == "تایید شده" || result == "قفل نشده") {
            $("#" + action).removeClass("badge-danger").addClass("badge-success");
            $("#btn" + action).removeClass("btn-suceess").addClass("btn-danger");
            if (result == "فعال")
                $("#btn" + action).html("غیرفعال شود");
            else if (result == "قفل نشده")
                $("#btn" + action).html("قفل شود");
            else
                $("#btn" + action).html("تایید نشود");
        }
        else {
            $("#" + action).removeClass("badge-success").addClass("badge-danger");
            $("#btn" + action).removeClass("btn-danger").addClass("btn-success");
            if (result == "غیرفعال")
                $("#btn" + action).html("فعال شود");
            else if (result == "قفل شده")
                $("#btn" + action).html("قفل نشود");
            else
                $("#btn" + action).html("تایید شود");
            $("#" + action).html(result);
        }
    });
}

function DeleteGroup(controllerName, btSelectItem) {
    $.ajax({
        url: '/Admin/' + controllerName + '/DeleteGroup?btSelectItem=' + btSelectItem,
        type: "post",
        data: {},
    }).done(function (result) {
        if (result != "Success")
            ShowSweetSuccessAlert(result);
        else
            ShowErrorMessage(result);
    });
}

function LikeOrDisLike(commentId, isLiked) {
    $.ajax({
        url: "/Admin/Api/v1/CommentApi/LikeOrDisLike?commentId=" + commentId + "&&isLike=" + isLiked,
    }).done(function (result) {
        $("#like").html(result.like);
        $("#dislike").html(result.dislike);
    });
}

function ActiveOrInActiveMembers(email) {
    $.ajax({
        url: "/Admin/Newsletter/ActiveOrInActiveMembers?email=" + email,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    }).done(function (result) {
        if (result == "Success")
            ShowSweetSuccessAlert(result);
        else
            ShowErrorMessage(result);
    });
}

function ConfirmOrInConfirm(commentId) {
    $.ajax({
        url: "/Admin/Comment/ConfirmOrInConfirm?commentId=" + commentId,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    }).done(function (result) {
        if (result == "Success")
            ShowSweetSuccessAlert(result);
        else
            ShowErrorMessage(result);
    });
}

function ConfirmOrInConfirmMessage(messageId) {
    $.ajax({
        url: "/Admin/MessageUser/ConfirmOrInConfirmMessage?messageId=" + messageId,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    }).done(function (result) {
        if (result == "Success")
            ShowSweetSuccessAlert(result);
        else
            ShowErrorMessage(result);
    });
}

function ChangeOrderState(orderId) {
    var e = document.getElementById("select_OrderId");
    var orderState = e.value;
    $.ajax({
        url: "/Admin/Order/ChangeOrderState?orderId=" + orderId + "&orderState=" + orderState,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    }).done(function (result) {
        if (result == "Success")
            ShowSweetSuccessAlert(result);
        else
            ShowErrorMessage(result);
    });
}

function IsPreferedOrNotPrefered(productId) {
    $.ajax({
        url: "/Admin/Product/IsPreferedOrNotPrefered?productId=" + productId,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    });
}

//$(function () {
//    window.setTimeout(function () {
//        debugger
//        var selectors = document.querySelectorAll('.select_ProductState');
//        selectors.forEach(i => i.addEventListener("change", (event) => {
//            debugger
//            var state = event.target.value;
//            var productId = event.target.previousSibling.pathname.replace("/Admin/Product/", "");           
//            $.ajax({
//                url: "/Admin/Product/ChangeProductState?productId=" + productId + "&productState=" + state,
//                beforeSend: function () { ShowLoading(); },
//                complete: function () { $("body").preloader('remove'); },
//                type: "get",
//                data: {},
//            }).done(function (result) {
//                if (result == 'success')
//                    ShowSweetSuccessAlert(result);
//                else if(result == 'error')
//                    ShowErrorMessage(result)
//            });
//        }));
//    }, 8000);
//})

function InsertGroupSpecialDiscount(insertGroup) {
    $.ajax({
        url: "/Admin/Discount/InsertGroupSpecialDiscount?insertGroup=" + insertGroup,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    }).done(function (result) {
        if (result == "Success")
            window.location.href = 'https://localhost:44309/Admin/Product/Index';
        else
            ShowErrorMessage(result);
    });
}

function DeleteDiscount(productId) {
    $.ajax({
        url: '/Admin/Discount/DeleteDiscount?productId=' + productId,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    }).done(function (result) {
        if (result == "Success")
            window.location.href = 'https://localhost:44309/Admin/Product/Index';
        else
            ShowSweetAlert(result);
    });
}

function IsRegisterOrNotRegister(sellerId) {
    $.ajax({
        url: "/Admin/Seller/IsRegisterOrNotRegister?sellerId=" + sellerId,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "get",
        data: {},
    });
}

function AddProductToStore(productId, sellerId, sellerProductIds) {
    $.ajax({
        url: "/Admin/Seller/AddGroup?productId=" + productId + '&sellerId=' + sellerId + '&sellerProductIds=' + sellerProductIds,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "post",
        data: {},
    }).done(function (result) {
        if (result == "Success")
            window.location.href = 'https://localhost:44309/Admin/Seller/ProductsOfSeller?sellerId=' + sellerId +'&isSeller=' + true;
        else
            ShowErrorMessage(result);
    });
}

function DeleteGroupProduct(btSelectItem) {
    $.ajax({
        url: '/Admin/Seller/DeleteGroupProduct?btSelectItem=' + btSelectItem,
        beforeSend: function () { ShowLoading(); },
        complete: function () { $("body").preloader('remove'); },
        type: "post",
        data: {},
    }).done(function (result) {
        ShowSweetAlert(result);
    });
}

function ShowProductSeller(sellerId) {
    window.location.href = 'https://localhost:44309/Admin/Seller/ProductsOfSeller?sellerId=' + sellerId + '&isSeller=' + true;
}

function ShowAllProduct(sellerId) {
    window.location.href = 'https://localhost:44309/Admin/Seller/ProductsOfSeller?sellerId=' + sellerId + '&isSeller=' + false;
}

function findImage() {
    return document.getElementById('modalImage').requestFullscreen();
}
