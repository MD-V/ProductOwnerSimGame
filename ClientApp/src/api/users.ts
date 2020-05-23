import service from '@/utils/request'
import {LoginRequest, LoginResponse, UserResponse } from '@/models/models'
import {ApiResponse } from '@/models/ApiResponse'

export const getUserInfo = async () =>
{
  return await service.request<UserResponse>({
    method: 'get',
    url: '/user/info'    
  })
    .then((response) => {
      
    let apiResponse = new ApiResponse<UserResponse>(true, response.data)
    
    return apiResponse
    })
    .catch(error => {
      console.log(error)
      return new ApiResponse<UserResponse>(false, undefined);
    })
}

export const login = async (username: string, password: string) =>
  {
    let loginRequest = new LoginRequest(username, password);

    return await service.request<LoginResponse>({
      method: 'post',
      url: '/user/login',
      data: loginRequest     
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<LoginResponse>(true, response.data)
      
      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<LoginResponse>(false, undefined);
      })
  }
