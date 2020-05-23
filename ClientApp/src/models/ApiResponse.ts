export class ApiResponse<T> {
    response?: T;
    success: boolean;
    
    constructor(success: boolean, response?: T) {
        this.response = response;
        this.success = success;
    }
}