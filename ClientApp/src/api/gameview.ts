import service from '@/utils/request'
import { GameView, Game} from '@/models/models'
import {ApiResponse } from '@/models/ApiResponse'

  export const getGameView = async (gameId: string) =>
  {
    let url = `/gameview/${gameId}`
    
    return await service.request<GameView>({
      method: 'get',
      url: url  
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<GameView>(true, response.data)

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<GameView>(false, undefined);
      })
  }