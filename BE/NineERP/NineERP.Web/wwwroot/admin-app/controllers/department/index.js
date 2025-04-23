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
            $('#image-input').click();
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
                                $('#crop-dimensions').text('Kích thước crop: 2039 x 243');
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
            $('#image-input-logo').click();
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
                                $('#crop-logo-dimensions').text('Kích thước crop: 250 x 250');
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
                    $('#btn-save-department').prop('disabled', true).text('Đang lưu...');
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
                    base.notify('Không thể kết nối đến máy chủ.', 'error');
                },
                complete: function () {
                    $('#btn-save-department').prop('disabled', false).text('Lưu');
                }
            });

        });
        //#endregion Handle Add/Edit Department
    }

    //#region Custom validation method for slug
    $.validator.addMethod("validSlug", function (value, element) {
        if (this.optional(element)) return true;
        return /^[a-z0-9\-]+$/.test(value);
    }, "Slug chỉ được chứa chữ thường không dấu, số và dấu gạch ngang (-).");
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
                    required: "Vui lòng nhập url.",
                    minlength: "Url tối thiểu 3 ký tự.",
                    maxlength: "Url tối đa 100 ký tự.",
                    validSlug: "Url không hợp lệ. Không được chứa dấu hoặc ký tự đặc biệt."
                },
                croppedImageLogo: {
                    required: "Vui lòng chọn logo cho phòng ban.",
                },
                name: {
                    required: "Tên phòng ban là bắt buộc.",
                    minlength: "Tên phòng ban quá ngắn (tối thiểu 2 ký tự).",
                    maxlength: "Tên phòng ban quá dài (tối đa 255 ký tự)."
                },
                email: {
                    required: "Email là bắt buộc.",
                    email: "Email không hợp lệ.",
                    maxlength: "Email không được vượt quá 255 ký tự."
                },
                phoneNumber: {
                    required: "Số điện thoại là bắt buộc.",
                    digits: "Vui lòng chỉ nhập số.",
                    minlength: "Số điện thoại tối thiểu 9 chữ số.",
                    maxlength: "Số điện thoại tối đa 15 chữ số."
                },
                address1: {
                    required: "Địa chỉ chính là bắt buộc.",
                    minlength: "Địa chỉ quá ngắn.",
                    maxlength: "Địa chỉ không được vượt quá 255 ký tự."
                },
                address2: {
                    maxlength: "Địa chỉ phụ không được vượt quá 255 ký tự."
                },
                faceBookUrl: {
                    url: "Đường dẫn Facebook không hợp lệ.",
                    maxlength: "Link Facebook không vượt quá 255 ký tự."
                },
                wikipediaUrl: {
                    url: "Đường dẫn Wikipedia không hợp lệ.",
                    maxlength: "Link Wikipedia không vượt quá 255 ký tự."
                },
                youtubeUrl: {
                    url: "Đường dẫn Youtube không hợp lệ.",
                    maxlength: "Link Youtube không vượt quá 255 ký tự."
                },
                croppedImage: {
                    required: "Vui lòng chọn ảnh hiển thị.",
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
        document.querySelector('label[for="image-input"]').innerText = 'Chọn file ảnh';
        if (cropper) {
            cropper.destroy();
            cropper = null;
        }
    }

    function clearCropLogo() {
        $('#image-preview-logo').attr('src', '/assets/images/250x250.svg');
        $('#croppedImageLogo').val('');
        document.getElementById('image-input-logo').value = '';
        document.querySelector('label[for="image-input-logo"]').innerText = 'Chọn file ảnh';
        if (cropperLogo) {
            cropperLogo.destroy();
            cropperLogo = null;
        }
    }
    //#endregion
};
