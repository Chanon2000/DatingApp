export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T> { // ในที่นี้ T หลักๆหน้าจะเป็น Member[]
    result: T; // T บนนั้นกับอันนี้จะเป็นclassเดียวกัน
    pagination: Pagination;
}