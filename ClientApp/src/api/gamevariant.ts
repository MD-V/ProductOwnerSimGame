import service from '@/utils/request'
import {GameVariantRequest, GameVariant  } from '@/models/models'
import {ApiResponse } from '@/models/ApiResponse'


export const getGameVariants = async (playerCount: number) =>
  {
    let loginRequest = new GameVariantRequest(playerCount);

    return await service.request<GameVariant[]>({
      method: 'post',
      url: '/gamevariant/search',
      data: loginRequest     
    })
      .then((response) => {

      let apiResponse = new ApiResponse<GameVariant[]>(true, response.data.map(x => Object.assign(new GameVariant(), x)))
      
      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<GameVariant[]>(false, undefined);
      })
  }
