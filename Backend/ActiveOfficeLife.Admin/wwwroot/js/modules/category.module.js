export class CategoryModule {
    constructor() {
        this.configApp = configInstance;
        this.spinner = spinnerInstance;
        this.messageApp = messageInstance;
        this.token = this.configApp.token;
        this.selectedRow = null;
        this.selectedData = null;
        this.currentPage = 1;
        this.pageSize = 10;
        this.sortField = 'name';
        this.sortDirection = 'asc';
        this.globalData = [];
        this.totalCount = 0;
    }

    async fetchData(sortField = 'name', sortDirection = 'asc', pageIndex = 1, pageSize = 10) {
        try {
            const url = new URL(this.configApp.urlApi + '/Category/all-paging');
            url.searchParams.append('sortField', sortField);
            url.searchParams.append('sortDirection', sortDirection);
            url.searchParams.append('pageIndex', pageIndex);
            url.searchParams.append('pageSize', pageSize);

            this.spinner.showFor("table-category");
            const res = await fetch(url.toString(), {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${this.token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!res.ok) {
                this.spinner.hideFor("table-category");
                throw new Error(`Lỗi ${res.status}: ${res.statusText}`);
            }

            const result = await res.json();
            this.globalData = result.data.items || result.data;
            this.totalCount = result.data.totalCount || this.globalData.length;
            this.renderTable(this.globalData);
            this.renderPagination(this.totalCount);
            this.spinner.hideFor("table-category");
            //this.messageApp.info("Dữ liệu đã được cập nhật.", {
            //    title: "Thành công",
            //    fullscreen: true,
            //    clickOutsideToClose: false
            //});
        } catch (error) {
            console.error('Lỗi gọi API:', error.message);
            this.showError('Không thể tải dữ liệu');
        }
    }

    renderTable(data) {
        const tbody = document.getElementById('userTableBody');
        tbody.innerHTML = '';

        data.forEach((cate, index) => {
            const row = document.createElement('tr');
            row.innerHTML = `
              <td>${(this.currentPage - 1) * this.pageSize + index + 1}</td>
              <td>${cate.name}</td>
              <td>${cate.slug}</td>
              <td>
                <button class="btn btn-success me-2" data-id="${cate.id}">✏️ Edit</button>
              </td>
            `;

            row.onclick = () => {
                if (this.selectedRow) this.selectedRow.classList.remove('selected');
                row.classList.add('selected');
                this.selectedRow = row;
                this.selectedData = cate;
            };

            const btn = row.querySelector('button');
            btn.addEventListener('click', (event) => {
                event.stopPropagation();
                this.editUser();
            });

            tbody.appendChild(row);
        });
    }

    renderPagination(totalItems) {
        const totalPages = Math.ceil(totalItems / this.pageSize);
        const container = document.getElementById('pagination');
        container.innerHTML = '';

        const prevBtn = document.createElement('li');
        prevBtn.className = `page-item ${this.currentPage === 1 ? 'disabled' : ''}`;
        prevBtn.innerHTML = `<a class="page-link" href="#">«</a>`;
        prevBtn.onclick = () => this.changePage(this.currentPage - 1);
        container.appendChild(prevBtn);

        for (let i = 1; i <= totalPages; i++) {
            const pageBtn = document.createElement('li');
            pageBtn.className = `page-item ${i === this.currentPage ? 'active' : ''}`;
            pageBtn.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            pageBtn.onclick = () => this.changePage(i);
            container.appendChild(pageBtn);
        }

        const nextBtn = document.createElement('li');
        nextBtn.className = `page-item ${this.currentPage === totalPages ? 'disabled' : ''}`;
        nextBtn.innerHTML = `<a class="page-link" href="#">»</a>`;
        nextBtn.onclick = () => this.changePage(this.currentPage + 1);
        container.appendChild(nextBtn);
    }

    changePage(page) {
        this.currentPage = page;
        this.fetchData(this.sortField, this.sortDirection, this.currentPage, this.pageSize);
    }

    onSort(field) {
        if (this.sortField === field) {
            this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortField = field;
            this.sortDirection = 'asc';
        }
        this.currentPage = 1;
        this.fetchData(this.sortField, this.sortDirection, this.currentPage, this.pageSize);
    }

    addUser() {
        this.messageApp.info('Hiện popup thêm mới (Add)');
    }

    editUser() {
        if (!this.selectedData) return alert('Vui lòng chọn dòng để sửa!');
        this.messageApp.info('Sửa người dùng: ' + this.selectedData.name);
    }

    refreshData() {
        this.fetchData();
        //this.messageApp.confirm("Bạn có muốn tiếp tục không?")
        //    .then(result => {
        //        if (result) {
        //            this.fetchData();
        //        } else {
        //            console.log("Người dùng đã chọn KHÔNG");
        //        }
        //    });
    }

    showError(message) {
        this.messageApp.info(message); // bạn có thể thay bằng Message.error nếu muốn
    }
}


