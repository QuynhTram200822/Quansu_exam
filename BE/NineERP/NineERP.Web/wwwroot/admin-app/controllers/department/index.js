var DepartmentController = function () {
    var cropper = null;
    this.initialize = function () {
        //loadData();
        registerEvents();
    };

    function registerEvents() {
        $("#ddl-show-page").on('change', function () {
            base.configs.pageSize = $(this).val();
            base.configs.pageIndex = 1;
            loadData(true);
        });

        $("#txtKeyword").on('keypress', function (e) {
            if (e.which === 13) loadData(true);
        });

        $("#btn-search").on('click', function () {
            base.configs.pageIndex = 1;
            loadData(true);
        });

        // Handle add department click
        $('#btn-add-department').on('click', function () {
            clearForm();
            $('#modalAddEditDepartment').modal('show');
        });

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
                                console.log('Cropper is ready', canvasData.width * (2039 / 243));
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

        // Apply crop
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
                $('#croppedImage').val(croppedImageDataUrl);

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

        // Cancel crop
        $('#crop-cancel').on('click', function () {
            clearCrop();
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
        });

        // Close crop modal with X button
        $('#crop-modal .close').on('click', function () {
            clearCrop();
            document.activeElement.blur();
            ('#crop-modal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                if ($('#modalAddEditDepartment').length) {
                    $('#modalAddEditDepartment').modal('show');
                    $('#modalAddEditDepartment').off('hidden.bs.modal'); // Ngăn kích hoạt crop-modal
                } else {
                    console.error('Modal #modalAddEditDepartment not found');
                }
            });
        });

        $('body').on('click', '.btn-delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');

            Swal.fire({
                title: window.localization?.ConfirmDeleteTitle || "Delete user?",
                text: window.localization?.ConfirmDeleteText || "Are you sure you want to delete this user?",
                type: "warning",
                showCancelButton: true,
                confirmButtonText: window.localization?.Yes || "Yes",
                cancelButtonText: window.localization?.No || "No"
            }).then(result => {
                if (result.value) {
                    $.post("/users/DeleteUser", { id }, function (response) {
                        base.notify(response.messages[0], response.succeeded ? 'success' : 'error');
                        if (response.succeeded) loadData(true);
                    });
                }
            });
        });

        $('body').on('submit', '#form-add-user', function (e) {
            e.preventDefault();
            const formData = $(this).serializeArray();
            const request = {};

            formData.forEach(field => {
                request[field.name] = field.value;
            });

            $.ajax({
                type: "POST",
                url: "/admin/users/Register",
                contentType: "application/json",
                data: JSON.stringify({ request: request, origin: base.getOrigin() }),
                success: function (response) {
                    if (response.succeeded) {
                        base.notify(response.messages[0], 'success');
                        $('#modal-add-user').modal('hide');
                        loadData(true);
                    } else {
                        base.notify(response.messages[0], 'error');
                    }
                },
                error: function () {
                    base.notify('Something went wrong.', 'error');
                }
            });
        });

    }

    function loadData(isPageChanged) {
        $.get("/admin/department/GetAllDepartmentPaging", {
            keyword: $('#txtKeyword').val(),
            roleName: $('#ddlRoleName').val(),
            pageSize: base.configs.pageSize,
            pageNumber: base.configs.pageIndex
        }, function (response) {
            const template = $('#table-template').html();
            let render = "";
            $("#lbl-total-records").text(response.totalCount);
            if (response.totalCount > 0) {
                let no = (base.configs.pageIndex - 1) * base.configs.pageSize + 1;
                $.each(response.data, function (i, item) {
                    render += Mustache.render(template, {
                        DisplayOrder: no++,
                        Email: item.email,
                        FullName: item.fullName,
                        Id: item.id,
                        AvatarUrl: item.avatarUrl
                            ? `<img src="${base.getOrigin()}${item.avatarUrl}" width="25" />`
                            : `<img src="/assets/images/users/no-avatar.png" width="25" />`,
                        CreatedOn: base.formatDateTime(item.createdOn),
                        Status: getUserStatus(item.lockoutEnabled, item.id)
                    });
                });
                $('#tbl-content').html(render);
                base.wrapPaging(response.totalCount, loadData, isPageChanged);
            } else {
                $('#tbl-content').html('<tr><td colspan="7" class="text-center text-muted">No records found.</td></tr>');
            }
        });
    }

    function getUserStatus(status, id) {
        const label = status
            ? (window.localization?.Block || "Block")
            : (window.localization?.Active || "Active");
        const btnClass = status ? 'btn-danger' : 'btn-success';
        return `<button class="btn btn-sm ${btnClass} btn-active" data-id="${id}">${label}</button>`;
    }

    function clearForm() {
        clearCrop() 
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
};
