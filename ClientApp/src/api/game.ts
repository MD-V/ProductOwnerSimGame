import service from '@/utils/request'
import {CreateGameRequest, CreateGameResponse, Game, JoinGameRequest, JoinGameResponse, StartGameResponse, CancelGameResponse, StartPhaseResponse, PhaseDoneResponse, SubmitDecisionResponse} from '@/models/models'
import {ApiResponse } from '@/models/ApiResponse'

export const createGame = async (gameVariantId: string) =>
  {
    let createGameRequest = new CreateGameRequest(gameVariantId);

    return await service.request<CreateGameResponse>({
      method: 'post',
      url: '/game',
      data: createGameRequest     
    })
      .then((response) => {

      let apiResponse = new ApiResponse<CreateGameResponse>(true, response.data)
      
      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<CreateGameResponse>(false, undefined);
      })
  }

  export const getGames = async () =>
  {
    return await service.request<Game[]>({
      method: 'get',
      url: '/game'     
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<Game[]>(true, response.data.map(x => Object.assign(new Game(), x)))

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<Game[]>(false, undefined);
      })
  }

  export const joinGame = async (accessCode: string) =>
  {
    let joinGameRequest = new JoinGameRequest(accessCode);
    
    return await service.request<JoinGameResponse>({
      method: 'post',
      url: '/game/join',  
      data: joinGameRequest   
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<JoinGameResponse>(true, response.data)

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<JoinGameResponse>(false, undefined);
      })
  }

  export const startGame = async (gameId: string) =>
  {
    let url = `/game/${gameId}/start`

    
    return await service.request<StartGameResponse>({
      method: 'post',
      url: url
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<StartGameResponse>(true, response.data)

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<StartGameResponse>(false, undefined);
      })
  }

  export const cancelGame = async (gameId: string) =>
  {
    let url = `/game/${gameId}/cancel`

    
    return await service.request<CancelGameResponse>({
      method: 'post',
      url: url
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<CancelGameResponse>(true, response.data)

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<CancelGameResponse>(false, undefined);
      })
  }

  export const submitStartPhase = async (gameId: string) =>
  {
    let url = `/game/${gameId}/startphase`

    
    return await service.request<StartPhaseResponse>({
      method: 'post',
      url: url
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<StartPhaseResponse>(true, response.data)

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<StartPhaseResponse>(false, undefined);
      })
  }

  export const submitPhaseDone = async (gameId: string) =>
  {
    let url = `/game/${gameId}/phasedone`

    
    return await service.request<PhaseDoneResponse>({
      method: 'post',
      url: url
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<PhaseDoneResponse>(true, response.data)

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<PhaseDoneResponse>(false, undefined);
      })
  }

  export const submitDecision = async (gameId: string, decisionId : string) =>
  {
    let url = `/game/${gameId}/submitdecision/${decisionId}`
    
    return await service.request<SubmitDecisionResponse>({
      method: 'post',
      url: url
    })
      .then((response) => {
        
      let apiResponse = new ApiResponse<SubmitDecisionResponse>(true, response.data)

      return apiResponse
      })
      .catch(error => {
        console.log(error)
        return new ApiResponse<SubmitDecisionResponse>(false, undefined);
      })
  }