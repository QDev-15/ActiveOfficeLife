import { apiInstance } from './core/api.module.js';
import { configInstance } from './core/config.module.js';
import { messageInstance } from './core/messages.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { utilities } from './core/utilities.module.js';

class AdModule {
    constructor() {
        this.configApp = configInstance;
        this.spinner = spinnerInstance;
        this.messageApp = messageInstance;
        this.token = configInstance.token;
        // paging init
        this.pageSize = 10;
        this.sortField = 'name';
        this.sortDirection = 'asc';
        this.globalData = [];
        this.totalCount = 0;

        this.modalAd = null;
        this.tableId = "adTable";
        this.tableInstance = null;
        this.Endpoints = {
            getAll: '/Ad/all',
            getById: '/Ad/getbyid',
            create: '/Ad/create',
            update: '/Ad/update',
            delete: '/Ad/delete'
        };
    }

    // init DataTable
    initTable() {
        this.tableInstance = $(`#${this.tableId}`).DataTable({
            processing: true,
            serverSide: true,
            stateSave: true,
            responsive: true,
            searchDelay: 500, // searchDelay mặc định sau 0.5s mới search
            order: [[1, 'desc']],
            pagingType: "full_numbers", // để hiển thị First, Prev, Next, Last
            mark: true, // ✅ Bật highlight search
            language: {
                paginate: {
                    first: '<i class="bi bi-chevron-double-left"></i>',
                    previous: '<i class="bi bi-chevron-left"></i>',
                    next: '<i class="bi bi-chevron-right"></i>',
                    last: '<i class="bi bi-chevron-double-right"></i>'
                }
            },
            ajax: (d, callback) => {
                let params = {
                    pageIndex: (d.start / d.length) + 1,
                    pageSize: d.length,
                    keySearch: d.search.value || "",
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].name : "name",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get('/Ad/all', params)
                    .then(response => {
                        this.pageSize = response.pageSize;
                        this.pageIndex = response.pageIndex;
                        callback({
                            data: response.items || [],
                            recordsTotal: response.totalCount || 0,
                            recordsFiltered: response.totalCount || 0,
                        });
                    })
                    .catch(error => {
                        console.error("❌ Error loading categories:", error);
                        callback({
                            data: [],
                            recordsTotal: 0,
                            recordsFiltered: 0
                        });
                    });
            },
            columns: [
                {
                    data: null,
                    title: "STT",
                    className: "text-center",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        return meta.row + 1 + meta.settings._iDisplayStart;
                    },
                    orderable: false // Không sort theo STT
                },
                {
                    data: "name",
                    name: "name",
                    title: "Ad Name",
                    className: "fw-bold text-primary",
                    orderable: true
                },
                {
                    data: "type",
                    name: "type",
                    title: "Type",
                    className: "text-center",
                    orderable: true,
                    //render: function (data) {
                    //    if (!data) return "";
                    //    return moment(data).format("DD/MM/YYYY HH:mm:ss");
                    //}
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Status",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (data.status) {
                            return '<span class="badge bg-success">Activated</span>';
                        } else {
                            return '<span class="badge bg-secondary">Deactivated</span>';
                        }
                    }
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Actions",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-warning btn-edit"
                                    onclick="adInstance.edit('${row.id}')"
                                    data-id="${row.id}"
                                    data-name="${row.name}">Edit</button>
                                <button class="btn btn-sm btn-danger btn-delete"
                                    onclick="adInstance.delete('${row.id}', '${row.name}')"
                                    data-id="${row.id}"
                                    data-name="${row.name}">Delete</button>
                                `;
                    }
                }
            ],
            pageLength: 10
        });

    }
    
    save() {
        if (!this._validateDom()) return;

        const model = this._collectForm();

        apiInstance.put(this.Endpoints.update, model)
            .then(() => {
                messageInstance.toastSuccess('Đã cập nhật quảng cáo.');
                //this.modalAd?.hide();
                this.refreshData();
            })
            .catch(err => {
                console.error('Lỗi cập nhật quảng cáo:', err);
                messageInstance.toastError('Không thể cập nhật quảng cáo.');
            });
    }

    add() {
        // call api create 
        apiInstance.post(this.Endpoints.create).then((resp) => {
            this.edit(null, resp);
        });
        
    }
   
    edit(id, model) {
        this.modalAd = this.configApp.createModal("adModal");
        $('#adTitle').text('Chi tiết quảng cáo');
        if (model) {
            // set form
            var formHtml = this._formHtml(model);
            $('#adBody').html(formHtml);
            this._bindInteractions();
        } else {
            apiInstance.get(this.Endpoints.getById, {id: id}).then((res) => {
                // set form
                var formHtml = this._formHtml(res);
                $('#adBody').html(formHtml);
                this._bindInteractions();
            }).catch((err) => {
                console.error('Lỗi tải quảng cáo: ', err);
                messageInstance.error('Không thể tải quảng cáo');
                return;
            });
        }
        this.modalAd.show();

    }
    delete(id, name) {
        var message = "Bạn có chắc chắn muốn xóa quảng cáo: <strong>\"" + name + "\"</strong> không?";
        this.messageApp.confirm(message).then((result) => {
            if (result) {
                apiInstance.delete('/Ad/delete?id=' + id)
                    .then(res => {
                        this.messageApp.success("Xóa thành công!");
                        this.refreshData();
                    })
                    .catch(err => {
                        console.error(err);
                        this.messageApp.error("Lỗi khi xóa quảng cáo!");
                    });
            };
        });
    }

    refreshData() {
        this.tableInstance?.ajax.reload();
    }

    // bind form
    _formHtml(ad = {}) {
        const id = ad.id ?? '';
        const name = ad.name ?? '';
        const type = ad.type ?? '';   // Banner | Text | ...
        const imageUrl = ad.imageUrl ?? '';
        const link = ad.link ?? '';
        const startDate = utilities._toInputDatetimeLocal(ad.startDate);
        const endDate = utilities._toInputDatetimeLocal(ad.endDate);
        const status = (ad.status ?? true) ? 'checked' : '';

        return `
        <form id="adForm" class="needs-validation" novalidate>
          <input type="hidden" id="ad_id" value="${id}">
          
          <div class="row g-3">
            <div class="col-md-8">
              <label for="ad_name" class="form-label">Tên quảng cáo</label>
              <input type="text" id="ad_name" class="form-control" value="${name}" required>
              <div class="invalid-feedback">Vui lòng nhập tên quảng cáo.</div>
            </div>

            <div class="col-md-4">
              <label for="ad_type" class="form-label">Vị trí</label>
              <input type="text" id="ad_type" class="form-control" value="${type}" required>
              <div class="invalid-feedback">Vui lòng nhập vị trí quảng cáo.</div>
            </div>

            <div class="col-md-8">
              <label for="ad_imageurl" class="form-label">Ảnh (URL)</label>
              <input type="url" id="ad_imageurl" class="form-control" placeholder="https://..." value="${imageUrl}">
              <div class="form-text">Dán link ảnh để xem preview.</div>
            </div>
            <div class="col-md-4 d-flex align-items-end">
              <img id="ad_image_preview" src="${imageUrl || ''}" alt="preview" class="img-fluid border rounded w-100" style="max-height:120px; object-fit:contain;">
            </div>

            <div class="col-12">
              <label for="ad_link" class="form-label">Liên kết khi click</label>
              <input type="url" id="ad_link" class="form-control" placeholder="https://..." value="${link}">
            </div>

            <div class="col-md-6">
              <label for="ad_start" class="form-label">Bắt đầu</label>
              <input type="datetime-local" id="ad_start" class="form-control" value="${startDate}" required>
              <div class="invalid-feedback">Vui lòng chọn thời gian bắt đầu.</div>
            </div>

            <div class="col-md-6">
              <label for="ad_end" class="form-label">Kết thúc</label>
              <input type="datetime-local" id="ad_end" class="form-control" value="${endDate}" required>
              <div class="invalid-feedback">Vui lòng chọn thời gian kết thúc.</div>
            </div>

            <div class="col-12">
              <div class="form-check form-switch">
                <input class="form-check-input" type="checkbox" id="ad_status" ${status}>
                <label class="form-check-label" for="ad_status">Kích hoạt</label>
              </div>
            </div>
          </div>
        </form>
      `;
    };
    _bindInteractions() {
        // Live preview ảnh
        const $img = document.querySelector('#ad_image_preview');
        const $url = document.querySelector('#ad_imageurl');
        if ($img && $url) {
            $url.addEventListener('input', () => { $img.src = $url.value || ''; });
        }
    };
    _collectForm() {
        const idEl = document.getElementById('ad_id');
        const nameEl = document.getElementById('ad_name');
        const typeEl = document.getElementById('ad_type');
        const imageUrlEl = document.getElementById('ad_imageurl');
        const linkEl = document.getElementById('ad_link');
        const startEl = document.getElementById('ad_start');
        const endEl = document.getElementById('ad_end');
        const statusEl = document.getElementById('ad_status');

        const id = idEl ? idEl.value || null : null;
        const name = nameEl ? (nameEl.value || '').trim() : '';
        const type = typeEl ? typeEl.value : '';
        const imageUrl = imageUrlEl ? (imageUrlEl.value || '').trim() : null;
        const link = linkEl ? (linkEl.value || '').trim() : null;
        const startStr = startEl ? startEl.value : '';
        const endStr = endEl ? endEl.value : '';
        const status = statusEl ? statusEl.checked : true;

        // Convert datetime-local -> ISO string (server dễ parse)
        const startDate = startStr ? new Date(startStr).toISOString() : null;
        const endDate = endStr ? new Date(endStr).toISOString() : null;

        return {
            id, name, type, imageUrl, link, startDate, endDate, status
        };
    };
    
    // validate Dom
    _validateDom() {
        const form = document.getElementById('adForm');
        if (!form) return false;

        // clear các trạng thái cũ
        const fields = [
            'ad_name', 'ad_type', 'ad_imageurl', 'ad_link', 'ad_start', 'ad_end'
        ].map(id => document.getElementById(id));
        fields.forEach(utilities._clearInvalid);

        // Bật Bootstrap validation mặc định
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return false;
        }

        // --- Lấy giá trị ---
        const nameEl = document.getElementById('ad_name');
        const typeEl = document.getElementById('ad_type');
        const imageUrlEl = document.getElementById('ad_imageurl');
        const linkEl = document.getElementById('ad_link');
        const startEl = document.getElementById('ad_start');
        const endEl = document.getElementById('ad_end');

        const name = (nameEl?.value || '').trim();
        const type = (typeEl?.value || '').trim();
        const imageUrl = (imageUrlEl?.value || '').trim();
        const link = (linkEl?.value || '').trim();
        const startStr = startEl?.value || '';
        const endStr = endEl?.value || '';

        // --- Kiểm tra nâng cao ---

        // 1) Tên quảng cáo
        if (!name) {
            utilities._markInvalid(nameEl, 'Vui lòng nhập tên quảng cáo.');
            nameEl?.focus();
            return false;
        }
        if (name.length > 150) {
            utilities._markInvalid(nameEl, 'Tên quảng cáo tối đa 150 ký tự.');
            nameEl?.focus();
            return false;
        }

        // 2) Loại quảng cáo
        if (!type) {
            utilities._markInvalid(typeEl, 'Vui lòng chọn loại quảng cáo.');
            typeEl?.focus();
            return false;
        }

        //// 3) Ảnh bắt buộc với một số loại
        //const needImageTypes = ['Banner', 'Popup', 'Video'];
        //if (needImageTypes.includes(type) && !imageUrl) {
        //    utilities._markInvalid(imageUrlEl, 'Loại này yêu cầu ảnh. Vui lòng nhập URL ảnh.');
        //    imageUrlEl?.focus();
        //    return false;
        //}

        // 4) URL hợp lệ (nếu có nhập)
        if (imageUrl && !utilities._isValidUrl(imageUrl)) {
            utilities._markInvalid(imageUrlEl, 'URL ảnh không hợp lệ.');
            imageUrlEl?.focus();
            return false;
        }
        if (link && !utilities._isValidUrl(link)) {
            utilities._markInvalid(linkEl, 'URL liên kết không hợp lệ.');
            linkEl?.focus();
            return false;
        }

        // 5) Thời gian: start < end
        if (startStr && endStr && new Date(startStr) > new Date(endStr)) {
            utilities._markInvalid(endEl, 'Thời gian kết thúc phải lớn hơn thời gian bắt đầu.');
            messageInstance?.warning?.('Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.');
            endEl?.focus();
            return false;
        }

         // (Tuỳ chọn) 6) Không cho kết thúc trong quá khứ
         const now = new Date();
         if (endStr && new Date(endStr) < now) {
             utilities._markInvalid(endEl, 'Thời gian kết thúc không được ở quá khứ.');
           endEl?.focus();
           return false;
         }

        // Passed
        form.classList.add('was-validated');
        return true;
    }
   
}

export const adInstance = new AdModule(); 

