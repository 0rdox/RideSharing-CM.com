export interface ApiResponse<T> {
  statusCode: number;
  message: string;
  data: T[] | T;
}

export interface loginReturnObject {
  bearerToken: string;
}
