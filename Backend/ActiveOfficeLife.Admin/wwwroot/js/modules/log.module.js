

export class LogModule {
    constructor() {
        this.config = configInstance;
        this.messageApp = messageInstance;
    }

    async fetchData(startDate, endDate, pageIndex = 1, pageSize = 10) {

        try {
            const url = new URL(this.config.urlApi + '/loger/all');
            url.searchParams.append('startDate', startDate);
            url.searchParams.append('endDate', endDate);
            url.searchParams.append('pageIndex', pageIndex);
            url.searchParams.append('pageSize', pageSize);

            const res = await fetch(url.toString(), {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${this.config.token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!res.ok) {
                throw new Error(`Lỗi ${res.status}: ${res.statusText}`);
            }

            const result = await res.json();
            globalData = result.data.items || result.data;         // Giả sử API trả về { data: { items: [], totalCount: 123 } }
            totalCount = result.data.totalCount || globalData.length;
            renderTable(globalData);
            renderPagination(totalCount);
        } catch (error) {
            console.error('Lỗi gọi API:', error.message);
            this.messageApp.error('Không thể tải dữ liệu');
        }
    }
}