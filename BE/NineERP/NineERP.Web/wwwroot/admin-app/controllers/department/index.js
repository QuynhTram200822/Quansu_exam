var DepartmentController = function () {
    var cropper = null;
    var cropperLogo = null;
    this.initialize = function () {
        loadData();
        validationForm();
        registerEvents();
    };
    function registerEvents() {

        //#region Handle Search And Show page
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $("#txtKeyword").on('keydown', function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                base.configs.pageIndex = 1;
                loadData(true);
            }
        });

        $("#btn-search").on('click', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });
        // #endregion Handle Search

        //#region Handle Upload and Crop Image Banner

        // Handle btn-select-image click
        $('#btn-select-image').on('click', function () {
            $('#image-input')[0].click();
        });

        // Handle image upload and cropping
        let isCropModalActive = false;
        $('#image-input').on('change', function (e) {
            var files = e.target.files;
            if (files && files.length > 0) {
                isCropModalActive = true;
                var file = files[0];
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#crop-container').html('<img id="image-to-crop" src="' + e.target.result + '" />');
                    document.activeElement.blur();
                    $('#crop-modal').modal('show');
                    var image = document.getElementById('image-to-crop');
                    if (cropper) {
                        cropper.destroy();
                    }

                    // Initialize cropper with a small delay to ensure image is loaded
                    setTimeout(function () {
                        cropper = new Cropper(image, {
                            aspectRatio: 2039 / 243,
                            viewMode: 1,
                            minCropBoxWidth: 300,
                            responsive: true,
                            guides: true,
                            center: true,
                            highlight: false,
                            background: false,
                            autoCropArea: 1.0,
                            cropBoxResizable: false,
                            dragMode: 'none',
                            ready: function () {
                                // Force crop box to cover entire image initially
                                var canvasData = cropper.getCanvasData();
                                // Adjust crop box to full canvas size while maintaining aspect ratio
                                cropper.setCropBoxData({
                                    left: canvasData.left,
                                    top: canvasData.top,
                                    width: canvasData.width,
                                    height: canvasData.width * (2039 / 243)
                                });
                                // Thông báo cho người dùng biết về kích thước crop
                                $('#crop-dimensions').text(localization.notificationCrop + ' 2039 x 243');
                            }
                        });
                    }, 300);
                };
                reader.readAsDataURL(file);

                $('#modalAddEditDepartment').modal('hide');
                $('#modalAddEditDepartment').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                    if (isCropModalActive) {
                        $('#crop-modal').modal('show');
                    }
                });
            }
        });

        // Apply crop banner
        $('#crop-apply').on('click', function () {
            if (cropper) {
                // Tính toán kích thước thực để giữ nguyên tỉ lệ
                // Nếu muốn giảm kích thước để tối ưu hóa
                var quality = 1.0; // 80% chất lượng, có thể điều chỉnh

                var canvas = cropper.getCroppedCanvas({
                    width: 2039,
                    height: 243,
                    fillColor: '#fff',
                    imageSmoothingEnabled: true,
                    imageSmoothingQuality: 'high'
                });

                var croppedImageDataUrl = canvas.toDataURL('image/jpeg', quality);
                $('#image-preview').attr('src', croppedImageDataUrl).show();
                $('#croppedImage').val(croppedImageDataUrl).trigger('change').valid();;

                cropper.destroy();
                cropper = null;

                document.activeElement.blur();
                $('#crop-modal').modal('hide');
                $('#crop-modal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                    if ($('#modalAddEditDepartment').length) {
                        $('#modalAddEditDepartment').modal('show');
                        $('#modalAddEditDepartment').off('hidden.bs.modal'); // Ngăn kích hoạt crop-modal
                    } else {
                        console.error('Modal #modalAddEditDepartment not found');
                    }
                });
            }
        });

        // Cancel crop banner
        $('#crop-cancel').on('click', function () {
            clearCrop();
            document.activeElement.blur();
            $('#crop-modal').modal('hide');
            $('#crop-modal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                if ($('#modalAddEditDepartment').length) {
                    $('#modalAddEditDepartment').modal('show');
                    $('#modalAddEditDepartment').off('hidden.bs.modal');
                } else {
                    console.error('Modal #modalAddEditDepartment not found');
                }
            });
        });

        // Close crop modal banner with X button
        $('#crop-modal .close').on('click', function () {
            clearCrop();
            document.activeElement.blur();
            ('#crop-modal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                if ($('#modalAddEditDepartment').length) {
                    $('#modalAddEditDepartment').modal('show');
                    $('#modalAddEditDepartment').off('hidden.bs.modal');
                } else {
                    console.error('Modal #modalAddEditDepartment not found');
                }
            });
        });

        //#endregion

        //#region Handle Upload and Crop Image Logo
        // Handle btn-select-image click
        $('#btn-select-image-logo').on('click', function () {
            $('#image-input-logo')[0].click();
        });
        // Handle image upload and cropping
        let isCropLogoModalActive = false;
        $('#image-input-logo').on('change', function (e) {
            var files = e.target.files;
            if (files && files.length > 0) {
                isCropLogoModalActive = true;
                var file = files[0];
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#crop-logo-container').html('<img id="image-to-crop-logo" src="' + e.target.result + '" />');
                    document.activeElement.blur();
                    $('#crop-logo-modal').modal('show');
                    var image = document.getElementById('image-to-crop-logo');
                    if (cropperLogo) {
                        cropperLogo.destroy();
                    }

                    // Initialize cropper with a small delay to ensure image is loaded
                    setTimeout(function () {
                        cropperLogo = new Cropper(image, {
                            aspectRatio: 250 / 250,
                            viewMode: 1,
                            minCropBoxWidth: 300,
                            responsive: true,
                            guides: true,
                            center: true,
                            highlight: false,
                            background: false,
                            autoCropArea: 1.0,
                            cropBoxResizable: false,
                            dragMode: 'none',
                            ready: function () {
                                // Force crop box to cover entire image initially
                                var canvasData = cropperLogo.getCanvasData();
                                // Adjust crop box to full canvas size while maintaining aspect ratio
                                cropperLogo.setCropBoxData({
                                    left: canvasData.left,
                                    top: canvasData.top,
                                    width: canvasData.width,
                                    height: canvasData.width * (2039 / 243)
                                });
                                // Thông báo cho người dùng biết về kích thước crop
                                $('#crop-logo-dimensions').text(localization.notificationCrop + '  250 x 250');
                            }
                        });
                    }, 300);
                };
                reader.readAsDataURL(file);

                $('#modalAddEditDepartment').modal('hide');
                $('#modalAddEditDepartment').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                    if (isCropLogoModalActive) {
                        $('#crop-logo-modal').modal('show');
                    }
                });
            }
        });

        // Apply crop logo
        $('#crop-logo-apply').on('click', function () {
            if (cropperLogo) {
                // Tính toán kích thước thực để giữ nguyên tỉ lệ
                // Nếu muốn giảm kích thước để tối ưu hóa
                var quality = 1.0;

                var canvas = cropperLogo.getCroppedCanvas({
                    width: 250,
                    height: 250,
                    fillColor: '#fff',
                    imageSmoothingEnabled: true,
                    imageSmoothingQuality: 'high'
                });

                var croppedImageDataUrl = canvas.toDataURL('image/jpeg', quality);
                $('#image-preview-logo').attr('src', croppedImageDataUrl).show();
                $('#croppedImageLogo').val(croppedImageDataUrl).trigger('change').valid();

                cropperLogo.destroy();
                cropperLogo = null;

                document.activeElement.blur();
                $('#crop-logo-modal').modal('hide');
                $('#crop-logo-modal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                    if ($('#modalAddEditDepartment').length) {
                        $('#modalAddEditDepartment').modal('show');
                        $('#modalAddEditDepartment').off('hidden.bs.modal');
                    } else {
                        console.error('Modal #modalAddEditDepartment not found');
                    }
                });
            }
        });

        // Cancel crop logo
        $('#crop-logo-cancel').on('click', function () {
            clearCropLogo();
            document.activeElement.blur();
            $('#crop-logo-modal').modal('hide');
            $('#crop-logo-modal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                if ($('#modalAddEditDepartment').length) {
                    $('#modalAddEditDepartment').modal('show');
                    $('#modalAddEditDepartment').off('hidden.bs.modal');
                } else {
                    console.error('Modal #modalAddEditDepartment not found');
                }
            });
        });

        // Close crop modal logo with X button
        $('#crop-logo-modal .close').on('click', function () {
            clearCropLogo();
            document.activeElement.blur();
            ('#crop-logo-modal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                if ($('#modalAddEditDepartment').length) {
                    $('#modalAddEditDepartment').modal('show');
                    $('#modalAddEditDepartment').off('hidden.bs.modal');
                } else {
                    console.error('Modal #modalAddEditDepartment not found');
                }
            });
        });
        //#endregion

        //#region Handle Add/Edit Department
        // Handle add department click
        $('#btn-add-department').on('click', function () {
            clearForm();
            $('#modalAddEditDepartment').find('.modal-title').text(localization.addDepartment);
            $("#modalAddEditDepartment").validate().resetForm();
            $('#modalAddEditDepartment').modal('show');
        });

        $('#btn-save-department').on('click', function (e) {
            e.preventDefault();
            if (!$('#form-add-department').valid()) {
                return;
            }

            var formData = {
                id: 0,
                name: $('[name="name"]').val(),
                slug: $('[name="slug"]').val(),
                email: $('[name="email"]').val(),
                phoneNumber: $('[name="phoneNumber"]').val(),
                address1: $('[name="address1"]').val(),
                address2: $('[name="address2"]').val(),
                faceBookUrl: $('[name="faceBookUrl"]').val(),
                wikipediaUrl: $('[name="wikipediaUrl"]').val(),
                youtubeUrl: $('[name="youtubeUrl"]').val(),
                logoUrl: $('#croppedImageLogo').val(),
                bannerUrl: $('#croppedImage').val(),
            };

            $.ajax({
                url: '/admin/department/add',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(formData),
                beforeSend: function () {
                    $('#btn-save-department').prop('disabled', true).text(localization.saving);
                },
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages, 'success');
                        $('#modalAddEditDepartment').modal('hide');
                        $('#form-add-department')[0].reset();
                    } else {
                        base.notify(response.messages, 'error');
                    }
                },
                error: function () {
                    base.notify(localization.notificationErrorAjax, 'error');
                },
                complete: function () {
                    $('#btn-save-department').prop('disabled', false).text(localization.save);
                }
            });

        });
        //#endregion Handle Add/Edit Department
    }

    //#region Custom validation method for slug
    $.validator.addMethod("validSlug", function (value, element) {
        if (this.optional(element)) return true;
        return /^[a-z0-9\-]+$/.test(value);
    }, localization.slugFormatRule);
    //#endregion

    //#region Handle Function validation form
    function validationForm() {
        $('#form-add-department').validate({
            ignore: [],
            lang: 'en',
            rules: {
                slug: {
                    required: true,
                    minlength: 3,
                    maxlength: 100,
                    validSlug: true
                },
                croppedImageLogo: {
                    required: function (element) {
                        return !$('#croppedImageLogo').val();
                    }
                },
                name: {
                    required: true,
                    minlength: 2,
                    maxlength: 255
                },
                email: {
                    required: true,
                    email: true,
                    maxlength: 255
                },
                phoneNumber: {
                    required: true,
                    digits: true,
                    minlength: 9,
                    maxlength: 15
                },
                address1: {
                    required: true,
                    minlength: 5,
                    maxlength: 255
                },
                address2: {
                    maxlength: 255
                },
                faceBookUrl: {
                    url: true,
                    maxlength: 255
                },
                wikipediaUrl: {
                    url: true,
                    maxlength: 255
                },
                youtubeUrl: {
                    url: true,
                    maxlength: 255
                },
                croppedImage: {
                    required: function (element) {
                        return !$('#croppedImage').val();
                    }
                }
            },
            messages: {
                slug: {
                    required: localization.slugRequired,
                    minlength: localization.slugMinLength,
                    maxlength: localization.slugMaxLength,
                    validSlug: localization.slugValid
                },
                croppedImageLogo: {
                    required: localization.croppedImageLogoRequired
                },
                name: {
                    required: localization.nameRequired,
                    minlength: localization.nameMinLength,
                    maxlength: localization.nameMaxLength
                },
                email: {
                    required: localization.emailRequired,
                    email: localization.emailInvalid,
                    maxlength: localization.emailMaxLength
                },
                phoneNumber: {
                    required: localization.phoneNumberRequired,
                    digits: localization.phoneNumberDigits,
                    minlength: localization.phoneNumberMinLength,
                    maxlength: localization.phoneNumberMaxLength
                },
                address1: {
                    required: localization.address1Required,
                    minlength: localization.address1MinLength,
                    maxlength: localization.address1MaxLength
                },
                address2: {
                    maxlength: localization.address2MaxLength
                },
                faceBookUrl: {
                    url: localization.facebookUrlInvalid,
                    maxlength: localization.facebookUrlMaxLength
                },
                wikipediaUrl: {
                    url: localization.wikipediaUrlInvalid,
                    maxlength: localization.wikipediaUrlMaxLength
                },
                youtubeUrl: {
                    url: localization.youtubeUrlInvalid,
                    maxlength: localization.youtubeUrlMaxLength
                },
                croppedImage: {
                    required: localization.croppedImageRequired
                }
            }
        });
    }
    //#endregion

    //#region Handle Function Load Data
    function loadData(isPageChanged) {
        $.ajax({
            url: "/admin/department/getAllDepartmentPaging",
            method: "GET",
            data: {
                keyword: $('#txtKeyword').val(),
                pageSize: base.configs.pageSize,
                pageNumber: base.configs.pageIndex
            },
            beforeSend: function () {
                base.startLoading();
            },
            success: function (response) {
                const template = $('#table-template').html();
                let render = "";
                $("#lbl-total-records").text(response.totalCount);
                if (response.totalCount > 0) {
                    let no = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
                    $.each(response.data, function (i, item) {
                        render += Mustache.render(template, {
                            DisplayOrder: no++,
                            Id: item.id,
                            Name: item.name,
                            PhoneNo: item.phoneNumber,
                            Email: item.email,
                            Url: item.slug,
                        });
                    });
                    $('#tbl-content').html(render);
                    base.wrapPaging(response.totalCount, loadData, isPageChanged);
                } else {
                    $('#tbl-content').html('<tr><td colspan="6" class="text-center text-muted">Không tìm thấy kết quả.</td></tr>');
                    base.wrapPaging(1, loadData, isPageChanged);
                }
            },
            error: function (xhr, status, error) {
                $('#tbl-content').html('<tr><td colspan="6" class="text-center text-danger">Không thể tải dữ liệu. Vui lòng thử lại.</td></tr>');
                base.wrapPaging(1, loadData, isPageChanged);
            },
            complete: function () {
                base.stopLoading();
            }
        });
    }
    //#endregion Load Data

    //#region Handle Functions clear form
    function clearForm() {
        clearCrop();
        clearCropLogo();
    }

    function clearCrop() {
        $('#image-preview').attr('src', '/assets/images/2039x243.svg');
        $('#croppedImage').val('');
        document.getElementById('image-input').value = '';
        document.querySelector('label[for="image-input"]').innerText = localization.selectImage;
        if (cropper) {
            cropper.destroy();
            cropper = null;
        }
    }

    function clearCropLogo() {
        $('#image-preview-logo').attr('src', '/assets/images/250x250.svg');
        $('#croppedImageLogo').val('');
        document.getElementById('image-input-logo').value = '';
        document.querySelector('label[for="image-input-logo"]').innerText = localization.selectImage;
        if (cropperLogo) {
            cropperLogo.destroy();
            cropperLogo = null;
        }
    }
    //#endregion
};
