import marked from 'marked'

export class LoginRequest {
    userNameOrEmail: string
    password: string

    constructor(userNameOrEmail: string, password: string) {
        this.userNameOrEmail = userNameOrEmail
        this.password = password
    }
  }

export class LoginResponse {
    token: string

    constructor(token: string) {
        this.token = token
    }
  }

  export class UserResponse {
    username: string
    email: string
    userid: string
    roles: string[]

    constructor(username: string, email: string,userid: string, roles: string[]) {
        this.username = username
        this.email = email
        this.userid = userid
        this.roles = roles
    }
  }

  export class GameVariantRequest {
    playerCount: number

    constructor(playerCount: number) {
        this.playerCount = playerCount
    }
  }

  export class GameVariantResponse {
    gameVariants!: GameVariant[]
  }

  export class GameVariant {
    gameVariantId!: string
    displayName!: string
    overallMarkDownText!: string
    playerCount!: number
  }

  export class CreateGameRequest {
    gameVariantId: string

    constructor(gameVariantId: string) {
      this.gameVariantId = gameVariantId
  }
  }

  export class CreateGameResponse {
    gameId!: string
    accessCode!: string
  }

  export class Game {
    gameId!: string
    accessCode!: string
    state!: string
    gameVariantName!: string
    gameVariantPlayerCount!: number
    currentJoinedPlayers!: number
  }

  export class JoinGameRequest {
    accessCode!: string

    constructor(accessCode: string) {
      this.accessCode = accessCode
    }
  }

  export const defaultJoinGameRequest: JoinGameRequest = {
    accessCode: '',
  }

  export class JoinGameResponse {
    gameId!: string
  }

  export class StartGameResponse {
    started!: boolean
  }

  export class CancelGameResponse {
    cancelled!: boolean
  }

  export class StartPhaseResponse {
    success!: boolean
  }

  export class PhaseDoneResponse {
    success!: boolean
  }

  export class SubmitDecisionResponse {
    success!: boolean
  }

  export enum GameRole {
    Unknown = 0,
    Stakeholder = 1,
    ProductOwner = 2
  }


  export class Decision {
    decisionId: string
    decisionMarkdownText: string

    get compiledecisionMarkdownText(): string { 
      return marked(this.decisionMarkdownText)
    }

    constructor(decisionId: string, decisionMarkdownText : string) {
      this.decisionId = decisionId
      this.decisionMarkdownText = decisionMarkdownText
    }
  }

  export interface GameView {
    decisionView: DecisionView
    gameFinishedView: GameFinishedView
    initialMissionView: InitialMissionView
    missionView: MissionView
    missionViewWithRemainingTime: MissionViewWithRemainingTime
    gameRole: GameRole
  }

  export interface DecisionView {
    decisions?: Decision[] | null
  }

  export interface GameFinishedView {
    phaseResults?: PhaseResult[] | null
    
    gameResult: GameResult
  }

  export class PhaseResult {
    id: number;
    decisionMarkdownText: string
    explanationMarkdownText: string
    decisionImpacts: DecisionImpact[]

    get compiledecisionMarkdownText(): string { 
      return marked(this.decisionMarkdownText)
    }
    get compiledexplanationMarkdownText(): string { 
      return marked(this.explanationMarkdownText)
    }

    constructor(id: number, decisionMarkdownText: string, explanationMarkdownText : string, decisionImpacts: DecisionImpact[]) {
      this.id = id
      this.decisionMarkdownText = decisionMarkdownText
      this.explanationMarkdownText = explanationMarkdownText
      this.decisionImpacts = decisionImpacts
    }
  }
  export interface DecisionImpact {
    impactCategory: string
    impact: Impact
  }
  export interface Impact {
    change: number
    oldValue: number
    newValue: number
  }
  export interface GameResult {
    decisionImpacts?: (DecisionImpact)[] | null
  }

  export interface InitialMissionView {
    overallMarkDownText: string
  }

  export interface MissionView {
    markDownText: string
    missionStarted: boolean
  }

  export interface MissionViewWithRemainingTime {
    markDownText: string
    remainingTime: number
    missionStarted: boolean
  }