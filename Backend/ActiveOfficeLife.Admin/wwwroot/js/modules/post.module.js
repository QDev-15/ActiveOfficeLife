import { apiInstance } from './core/api.module.js';
import { mvcInstance } from './core/mvc.module.js';
import { configInstance } from './core/config.module.js';
import { messageInstance } from './core/messages.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { dialogInstance } from './core/dialog.module.js';
import { utilities } from './core/utilities.module.js';
import { initCK, updateSourceOnSubmit } from './core/ckeditor.module.js';
class PostModule {

    constructor() {
        this.token = configInstance.token;
        this.currentPage = 1;
        this.pageSize = 10;
        this.sortField = 'name';
        this.sortDirection = 'asc';
        this.globalData = [];
        this.totalCount = 0;
        this.tableId = "articlesTable";
        this.postTableInstance = null;
        this.ENDPOINTS = {
            GET_ONE: (id) => `/post/get/${id}`,        // chỉnh cho đúng route API của bạn
            CAT_ALL: `/category/all?pageSize=10000`, // hoặc /Category/all
            TAG_ALL: `/tag/all?pageSize=10000`,
            CREATE: `/post/create`,
            UPDATE: `/Post/update`,
            PATCH: `/Post/patch`,
            DELETE: (id) => `/post/delete/${id}`,     // nếu API của bạn là /post/{id} thì sửa lại
        };
        this.id = null;
        this.form = null;
    }

    // init DataTable
    initTable() {
        this.postTableInstance = $(`#${this.tableId}`).DataTable({
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
                    status: $("#article_status").val(),
                    pageIndex: (d.start / d.length) + 1,
                    pageSize: d.length,
                    keySearch: d.search.value || "",
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].name : "name",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get('/post/all', params)
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
                    data: "title",
                    name: "title",
                    title: "Article Name",
                    className: "fw-bold text-primary",
                    orderable: true
                },
                {
                    data: "slug",
                    name: "slug",
                    title: "Slug",
                    className: "text-center",
                    orderable: true,
                    //render: function (data) {
                    //    if (!data) return "";
                    //    return moment(data).format("DD/MM/YYYY HH:mm:ss");
                    //}
                },
                {
                    data: null, // null để lấy toàn bộ row
                    name: "category",
                    title: "Category name",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (data.category && data.category.name) {
                            return data.category.name;
                        } else {
                            return '----';
                        }
                    }
                },
                {
                    data: "status",
                    name: "status",
                    title: "Status",
                    className: "text-center",
                    orderable: true,
                    render: (status) => {
                        const s = (status || '').toString().toLowerCase();
                        const map = {
                            'published': 'success',
                            'draft': 'secondary',
                            'pending': 'warning',
                            'closed': 'dark'
                        };
                        const color = map[s] || 'secondary';
                        const text = status || 'Unknown';
                        return `<span class="badge bg-${color}">${text}</span>`;
                    }
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Actions",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-warning btn-edit"
                                    onclick="postInstance.edit('${row.id}', '${row.title}')"
                                    data-id="${row.id}"
                                    data-name="${row.title}">Edit</button>
                                <button class="btn btn-sm btn-danger btn-delete"
                                    onclick="postInstance.delete('${row.id}', '${row.title}')"
                                    data-id="${row.id}"
                                    data-name="${row.title}">Delete</button>
                                `;
                    }
                }
            ],
            pageLength: 10
        });

    }

    // ====== Life cycle ======
    initEditPage() {
        // auto-Select2 toàn site (đã có util auto)
        utilities.setupSelect2Auto?.();

        // lấy id từ dataset hoặc query
        const root = document.getElementById('post-edit-root');
        this.id = root?.dataset?.postId || new URLSearchParams(location.search).get('id');
        if (!this.id) {
            messageInstance.error('Không tìm thấy ID bài viết.');
            return;
        }
        this.loadData();

    }
    refreshData() {
        this.postTableInstance?.ajax?.reload(null, false);
    }

    edit(id) {
        // Điều hướng tới trang Edit. Nếu action Edit nhận query ?id=...
        window.location.href = `/Articles/Edit?id=${encodeURIComponent(id)}`;
        // Nếu bạn map theo route /Articles/Edit/{id} thì dùng:
        // window.location.href = `/Articles/Edit/${encodeURIComponent(id)}`;
    }
    async loadData() {
        try {
            spinnerInstance.show?.();
            $('#content-artile-edit').html('');
            mvcInstance.get('/Articles/form?id=' + this.id)
                .then(html => {
                    $('#content-artile-edit').html(html);
                    this.form = configInstance.initValidatorForm("content-artile-edit");
                    var statusElement = document.getElementById('Status');
                    // status text
                    const statusText = document.getElementById('statusText');
                    statusText.textContent = this.statusDisplay(statusElement.value);
                    // category
                    const categoryId = $('#CategoryId').val();
                    const $cat = $('#CategoryId');
                    if ($cat.length) {
                        $cat.val(categoryId || '').trigger('change');
                    }
                    this.wireInputs();
                    // render button
                    this.renderButtons(statusElement.value);
                    utilities.lastLoadedAt = Date.now(); // phục vụ auto-reload helper nếu có
                    initck('#Content').then(editor => {
                        // Đồng bộ nội dung khi submit form
                        const form = document.querySelector('form'); // form hiện tại
                        if (form) updateSourceOnSubmit(form);
                    }).catch(console.error);;
                }).catch((err) => {
                    console.error('Lỗi tải form', err);
                    messageInstance.error('Không thể tải form');
                });
            
        } catch (e) {
            console.error(e);
            messageInstance.error(e?.message || 'Tải dữ liệu thất bại');
        } finally {
            spinnerInstance.hide?.();
        }
    }
    validate() {
        return this.form?.valid();
    }
    // ====== Render ======

    renderButtons(status) {
        // ẩn tất cả
        const hide = id => document.getElementById(id)?.classList.add('d-none');
        const show = id => document.getElementById(id)?.classList.remove('d-none');
        ['btnView', 'btnPublish', 'btnUnpublish', 'btnClose', 'btnRepublish'].forEach(hide);

        const s = (status || '').toString().toLowerCase();
        if (s === 'draft') {
            show('btnPublish');
        } else if (s === 'published') {
            show('btnView'); show('btnUnpublish'); show('btnClose');
        } else if (s === 'paused') {
            show('btnRepublish'); show('btnClose');
        } else if (s === 'closed') {
            show('btnRepublish');
        }
    }

    statusDisplay(s) {
        const map = {
            draft: 'Bản nháp',
            published: 'Đã xuất bản',
            paused: 'Tạm dừng',
            closed: 'Đóng',
        };
        return map[(s || '').toLowerCase()] || (s || 'Không rõ');
    }
    // validate Title
    blurTitle() {
        this.validate();
        this.setAutoSlug();
    }
    // validate Slug
    setAutoSlug = () => {
        const $title = document.getElementById('Title');
        const $slug = document.getElementById('Slug');
        const next = utilities.slugify($title.value);
        if (!next) return;
        const userEdited = $slug.dataset.userEdited === 'true';
        if (!userEdited || !$slug.value) {
            $slug.value = next;
            $slug.dataset.lastAuto = next;  // ghi nhớ bản auto gần nhất
        }
    };

    reSlug() {
        const $slug = document.getElementById('Slug');
        $slug.dataset.userEdited = 'false';
        this.setAutoSlug();
    }
    changeSlug() {
        const $slug = document.getElementById('Slug');
        const lastAuto = $slug.dataset.lastAuto || '';
        $slug.dataset.userEdited = String($slug.value !== lastAuto);
    }
    // end validate Slug
    wireInputs() {
        const $summary = document.getElementById('Summary');
        $summary?.addEventListener('input', () => this.updateSummaryCounter());

        ['SeoMetadata_MetaTitle', 'SeoMetadata_MetaDescription'].forEach(id => {
            document.getElementById(id)?.addEventListener('input', () => this.updateSeoCounters());
        });

        // Khi Title blur, nếu SeoMetadata_MetaTitle đang trống -> gợi ý
        document.getElementById('Title')?.addEventListener('blur', () => {
            const el = document.getElementById('SeoMetadata_MetaTitle');
            if (el && !el.value?.trim()) el.value = document.getElementById('Title').value || '';
            this.updateSeoCounters();
        });
        // run first, after reload
        this.updateSummaryCounter();
        this.updateSeoCounters();
    }
    updateSummaryCounter() {
        const $summary = document.getElementById('Summary');
        const $counter = document.getElementById('summaryCounter');
        if ($summary && $counter) $counter.textContent = ($summary.value || '').length;
    }
    updateSeoCounters() {
        const t = (document.getElementById('SeoMetadata_MetaTitle')?.value || '').length;
        const d = (document.getElementById('SeoMetadata_MetaDescription')?.value || '').length;
        const tEl = document.getElementById('seoTitleCounter');
        const dEl = document.getElementById('seoDescCounter');
        if (tEl) tEl.textContent = t;
        if (dEl) dEl.textContent = d;
    }
    // ====== CRUD via API ======
    gatherPayload() {
        // gom dữ liệu từ form → payload API
        const tags = Array.from(document.querySelectorAll('#tagsContainer input[type="checkbox"]:checked')).map(x => x.value);
        const seoId = document.getElementById('SeoMetadataId')?.value || null;
        const metaT = document.getElementById('SeoMetadata_MetaTitle')?.value?.trim() || null;
        const metaD = document.getElementById('SeoMetadata_MetaDescription')?.value?.trim() || null;
        const metaK = document.getElementById('SeoMetadata_MetaKeywords')?.value?.trim() || null;
        return {
            id: this.id,
            title: document.getElementById('Title').value?.trim(),
            slug: document.getElementById('Slug').value?.trim(),
            summary: document.getElementById('Summary').value?.trim(),
            content: document.getElementById('Content').value?.trim(),
            categoryId: $('#CategoryId').val() || null,
            tagIds: tags, // hoặc Tags: [{id},…] tùy API
            // ===== SEO =====
            seoMetadataId: seoId,  // để server biết record hiện có
            seoMetadata: {
                id: seoId,                     // có thể null nếu tạo mới
                metaTitle: metaT,
                metaDescription: metaD,
                metaKeywords: metaK
            }
        };

    }
    async add() {
        try {
            // Gửi body rỗng: backend sẽ set AuthorId = current user, Status = Draft
            const created = await apiInstance.post(this.ENDPOINTS.CREATE, {});
            const newId = created?.id || created?.Id; // tuỳ casing server trả về

            if (!newId) {
                messageInstance.error('Không lấy được ID bài viết vừa tạo.');
                return;
            }

            // Chuyển sang trang Edit
            this.edit(newId);
        } catch (e) {
            console.error('Create post failed:', e);
            messageInstance.error(e?.message || 'Tạo bài viết thất bại.');
        }
    }
    async save() {
        try {
            spinnerInstance.show?.();
            const body = this.gatherPayload();
            // Nếu API của bạn phân biệt UPDATE vs PATCH, chọn cái bạn muốn:
            const res = await apiInstance.put?.(this.ENDPOINTS.UPDATE, body)
                ?? await apiInstance.post(this.ENDPOINTS.UPDATE, body);
            messageInstance.success('Đã lưu bài viết.');
            // reload lại để đồng bộ timestamp/status nếu server thay đổi
            await this.loadData();
        } catch (e) {
            console.error(e);
            messageInstance.error(e?.message || 'Lưu thất bại.');
        } finally {
            spinnerInstance.hide?.();
        }
    }

    async changeStatus(nextStatus) {
        try {
            spinnerInstance.show?.();
            const res = await apiInstance.patch(this.ENDPOINTS.PATCH, { id: this.id, status: nextStatus });
            messageInstance.success('Trạng thái đã cập nhật.');
            await this.loadData();
        } catch (e) {
            console.error(e);
            messageInstance.error(e?.message || 'Đổi trạng thái thất bại.');
        } finally {
            spinnerInstance.hide?.();
        }
    }

    async delete() {
        try {
            if (!confirm('Xóa bài viết này?')) return;
            spinnerInstance.show?.();
            await apiInstance.delete?.(this.ENDPOINTS.DELETE(this.id))
                ?? await apiInstance.post(this.ENDPOINTS.DELETE(this.id)); // tùy API
            messageInstance.success('Đã xóa.');
            this.back();
        } catch (e) {
            console.error(e);
            messageInstance.error(e?.message || 'Xóa thất bại.');
        } finally {
            spinnerInstance.hide?.();
        }
    }

    // ====== Nav helpers ======
    back() { location.href = '/Articles?s=' + document.getElementById('Status').value; }
    backToDetail(id) {

    }
    view() {
        const id = this.id;
        const slug = (document.getElementById('Slug')?.value || this.model?.slug || '').trim();
        const s = (this.model?.status || '').toLowerCase();

        //// Nếu có slug -> đi public URL; nếu chưa publish thì thêm ?preview=1
        //if (slug) {
        //    const query = s === 'published' ? '' : '?preview=1';
        //    window.open(`/bai-viet/${encodeURIComponent(slug)}${query}`, '_blank');
        //    return;
        //}

        // Fallback theo id (preview)
        //window.open(`/Articles/View/${encodeURIComponent(id)}?preview=1`, '_blank');
        window.location.href = `/Articles/View/${encodeURIComponent(id)}?preview=1`;
    }


   

}
export const postInstance = new PostModule(); 
