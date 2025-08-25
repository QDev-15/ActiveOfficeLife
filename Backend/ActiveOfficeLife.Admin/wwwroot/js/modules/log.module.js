import { configInstance } from "./core/config.module.js";
import { messageInstance } from "./core/messages.module.js";
import { apiInstance } from "./core/api.module.js";
import { spinnerInstance } from "./core/spinner.module.js"
import { dateHelper } from "./core/dateHelper.js"; 


class LogModule {
    constructor() {
        this.config = configInstance;
        this.messageApp = messageInstance;
        this.currentPage = 1;
        this.pageIndex = 1;
        this.pageSize = 10;
        this.globalData = [];
        this.totalCount = 0;
        this.tableId = "logsTable";
        this.tableInstance = null;
    }
    initTable() {
        this.tableInstance = $(`#${this.tableId}`).DataTable({
            processing: true,
            serverSide: true,
            stateSave: true,
            responsive: true,
            searchDelay: 500, // searchDelay mặc định sau 0.5s mới search
            order: [[1, 'desc']],
            pagingType: "full_numbers", // để hiển thị First, Prev, Next, Last
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
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].data : "name",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get('/loger/all', params)
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
                    data: "message",
                    title: "Message",
                    className: "fw-bold text-primary col-message",
                    orderable: true
                },
                {
                    data: "level",
                    title: "level",
                    className: "text-center",
                    orderable: true,
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Date",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `${ dateHelper.formatDefault(data.timestamp) }`;
                    }
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Actions",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-success me-2" logInstance.view('${data.id}'">View</button>`;
                    }
                }
            ],
            pageLength: 10
        });

    }

    view(id) {
        
    }

    refesh() {
        this.tableInstance?.ajax.reload(null, false);
    }
}

export const logInstance = new LogModule();