<template>
  <div class="app-container">     
    <transition name="fade">
      <div v-if="decisionView !== null">
        <h4>{{ $t('game.makeDecision') }}</h4>       
        <el-divider></el-divider>
        <el-table
          ref="singleTable"
          :data="decisions"
          highlight-current-row
          @current-change="handleCurrentChange"
          :show-header="false"
          style="width: 100%">
            <el-table-column
              type="index"
              label="ID"
              width="50">
            </el-table-column>
            <el-table-column
              property="compiledecisionMarkdownText"
              label="Name"
              >
              <template scope="scope">
                <span v-html="scope.row.compiledecisionMarkdownText"></span>
              </template>
            </el-table-column>
        </el-table>
        <el-divider></el-divider>
        <el-button
            v-loading="saveDecisionButtonLoading"
            :disabled="selectedDecision === null"
            type="success"
            @click="saveDecisionButtonClicked">
            {{ $t('game.saveDecision') }}
        </el-button>
      </div>
    </transition>
    <transition name="fade">
      <div v-if="initialMissionView !== null">
        <h4>{{ $t('game.InitialMissionScreen') }}</h4>
        <el-divider></el-divider>
        <el-row>
          <div>
            {{ $t('game.roleText') }} <b>{{$t('game.role' + gameRole )}}</b>
          </div>
          </el-row>
        <el-row>
          <div v-html="compiledInitialMissionMarkdown"></div>
          </el-row>
        <el-divider></el-divider>
        <el-row>
          <el-col>
            <el-button
              v-loading="initialMissionButtonLoading"
              :disabled="initialMissionButtonDisabled"
              type="success"
              @click="initialMissionReadyButtonClicked">
              {{ $t('game.ready') }}
            </el-button>
          </el-col>
          <el-col v-if="initialMissionButtonDisabled">       
            <el-divider></el-divider>
            {{ $t('game.waitingForOtherPlayers') }}
          </el-col>    
        </el-row>
      </div>
    </transition>
    <transition name="fade">
      <div v-if="missionView !== null || missionViewWithRemainingTime !== null">
        <h4>{{ $t('game.missionScreen') }}</h4>
        <el-divider></el-divider>
        <el-row>
          <div>
            {{ $t('game.roleText') }} <b>{{$t('game.role' + gameRole )}}</b>
          </div>
          </el-row>
        <el-row>
          <div v-html="compiledMissionMarkdown"></div>
        </el-row>     
        <el-row v-if="remainingTime >0">
          <el-divider></el-divider>
          <div>{{$t('game.timeRemaining')+ ' ' + remainingTime + ' '+ $t('game.seconds')}}</div>
        </el-row>  
        <el-row v-if="!isMissionStarted && remainingTime === 0">
            <el-divider></el-divider>
            <el-button
              v-loading="missionButtonReadyLoading"
              :disabled="missionButtonReadyDisabled"
              type="success"
              @click="missionReadyButtonClicked">
              {{ $t('game.ready') }}
            </el-button>
        </el-row>     
        <el-row  v-if="missionButtonReadyDisabled && remainingTime === 0 && !(isMissionStarted && gameRole !== undefined && gameRole === 2)"> 
          <el-divider></el-divider>
          {{ $t('game.waitingForOtherPlayers') }}
        </el-row>
        <el-row v-if="isMissionStarted && gameRole !== undefined && gameRole === 2">
            <el-divider></el-divider>
            <el-button
              v-loading="missionButtonPhaseDoneLoading"
              type="success"
              @click="missionPhaseDoneButtonClicked">
              {{ $t('game.phaseDone') }}
            </el-button>
        </el-row>
      </div>
    </transition>
    <transition name="fade">
      <div v-if="gameFinishedView !== null">
        <h4>{{ $t('game.gameResults') }}</h4>
        <el-divider></el-divider>
        <el-row>
          <div>
            {{ $t('game.roleText') }} <b>{{$t('game.role' + gameRole )}}</b>
          </div>
        </el-row>
        <el-divider></el-divider>
        <el-collapse v-model="activeResultView">
          <el-collapse-item :title="$t('game.overallResults')" name="1">
            <el-table
              :data="gameFinishedView.gameResult.decisionImpacts"
              style="width: 100%">
                <el-table-column
                  :label="$t('game.category')">
                  <template scope="scope">
                    <span>{{$t('game.'+ scope.row.impactCategory)}}</span>
                  </template>
                </el-table-column>
                <el-table-column
                  property="impact.oldValue"
                  :label="$t('game.oldValue')">
                </el-table-column>
                <el-table-column
                  property="impact.newValue"
                  :label="$t('game.newValue')">
                </el-table-column>
                <el-table-column
                  property="impact.change"
                  :label="$t('game.change')">
                </el-table-column>
              </el-table>
          </el-collapse-item>
          <el-collapse-item :title="$t('game.phaseResults')" name="2">
            <el-row v-for="item in gameFinishedView.phaseResults" :key="item.index">
              <div v-html="item.compiledecisionMarkdownText"></div>
              <el-table
              :data="item.decisionImpacts"
              style="width: 100%">
                <el-table-column
                  :label="$t('game.category')">
                  <template scope="scope">
                    <span>{{$t('game.'+ scope.row.impactCategory)}}</span>
                  </template>
                </el-table-column>
                <el-table-column
                  property="impact.oldValue"
                  :label="$t('game.oldValue')">
                </el-table-column>
                <el-table-column
                  property="impact.newValue"
                  :label="$t('game.newValue')">
                </el-table-column>
                <el-table-column
                  property="impact.change"
                  :label="$t('game.change')">
                </el-table-column>
              </el-table>
              <div v-html="item.compiledexplanationMarkdownText"></div>
            <el-divider></el-divider>
            </el-row>
          </el-collapse-item>
        </el-collapse>  
        <el-divider></el-divider>
        <el-row>
          <el-button
            type="success"
            @click="backToOverview">
            {{ $t('route.backToDashboard') }}
          </el-button>
        </el-row>
      </div>
    </transition>
  </div>
  
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { submitStartPhase, submitPhaseDone, submitDecision } from '@/api/game'
import { getGameView } from '@/api/gameview'
import { Game, GameView, InitialMissionView, MissionViewWithRemainingTime, MissionView, GameFinishedView, DecisionView, GameRole, Decision, PhaseResult } from '@/models/models'
import { UserModule } from '@/store/modules/user'
import {HubConnectionBuilder, LogLevel, HubConnection} from '@microsoft/signalr'
import { DecisionViewComponent, GameFinishedViewComponent, InitialMissionViewComponent, MissionViewComponent, MissionViewWithRemainingTimeComponent, TestComponent } from "./components";
import marked from 'marked'

@Component({
  name: 'PlayGame',
  components: {
    UserModule,
    DecisionViewComponent, 
    GameFinishedViewComponent, 
    InitialMissionViewComponent, 
    MissionViewComponent, 
    MissionViewWithRemainingTimeComponent,TestComponent
  }
})
export default class extends Vue {
  
  activeResultView: string[] =  ['1']

  salary:number = 20

  private connection!: HubConnection

  private gameId : string = ''
  
  gameRole: GameRole = GameRole.ProductOwner

  missionView: MissionView | null = null
  missionViewWithRemainingTime: MissionViewWithRemainingTime | null= null
  missionMarkDownText : string = ""
  missionButtonReadyLoading : boolean = false
  missionButtonReadyDisabled : boolean = false
  missionButtonPhaseDoneLoading : boolean = false
  isMissionStarted: boolean = false
  remainingTime : number = 0

  get compiledMissionMarkdown() {
    return marked(this.missionMarkDownText);
  }

  initialMissionView: InitialMissionView | null = null
  initialMissionOverallMarkDownText : string = ""
  initialMissionButtonLoading : boolean = false
  initialMissionButtonDisabled : boolean = false

 get compiledInitialMissionMarkdown() {
    return marked(this.initialMissionOverallMarkDownText);
  }

  decisionView: DecisionView | null = null
  saveDecisionButtonLoading : boolean = false
  decisions : Decision[] = []

  selectedDecision : Decision | null = null

  gameFinishedView: GameFinishedView | null = null


  async created() {   
    this.gameId = this.$route.params && this.$route.params.gameId

    const token = `${UserModule.token}`
    
    this.salary = 50

    let currentGameViewResponse = await getGameView(this.gameId)
    
    if(currentGameViewResponse.success && currentGameViewResponse.response !== undefined)
    {
      this.setNewGameView(currentGameViewResponse.response);
    }

    let url = process.env.VUE_APP_SIGNALR_URL + "/gameviewupdates"

    this.connection = new HubConnectionBuilder()
                .withUrl(url, {
                  accessTokenFactory: () => token
                })
                .configureLogging(LogLevel.Error)
                .withAutomaticReconnect()
                .build();

    this.connection.on("UpdateGameViewAsync", (gameView : GameView) => {
      this.setNewGameView(gameView);
    });

    this.connection.onreconnected((connectionId)=> {
      console.log(connectionId);
    });

    this.connection.start()
  }
  
  private async initialMissionReadyButtonClicked() {
    this.initialMissionButtonLoading = true
    
    let startPhaseResponse = await submitStartPhase(this.gameId)

    if(!startPhaseResponse.success || startPhaseResponse.response === undefined )
    {
      //TODO show error
    }
    else
    {
      this.initialMissionButtonDisabled = true
    }

    this.initialMissionButtonLoading = false
  }

  private async missionReadyButtonClicked() {
    this.missionButtonReadyLoading = true;
    
    let startPhaseResponse = await submitStartPhase(this.gameId)

    if(!startPhaseResponse.success || startPhaseResponse.response === undefined )
    {
      //TODO show error
    }
    else
    {
      this.missionButtonReadyDisabled = true
    }

    this.missionButtonReadyLoading = false
  }

  private async missionPhaseDoneButtonClicked() {
    this.missionButtonPhaseDoneLoading = true;
    
    let startPhaseResponse = await submitPhaseDone(this.gameId)

    if(!startPhaseResponse.success || startPhaseResponse.response === undefined )
    {
      //TODO show error
    }

    this.missionButtonPhaseDoneLoading = false
  }

  setNewGameView(gameView: GameView) {
    this.resetCurrentView()

    if(this.gameRole === undefined || this.gameRole != gameView.gameRole)
    {
      this.gameRole = gameView.gameRole
    }

    if(gameView.initialMissionView != null)
    {
      let initialMissionView = gameView.initialMissionView

      this.initialMissionOverallMarkDownText = initialMissionView.overallMarkDownText

      this.initialMissionView = initialMissionView
    }
    else if(gameView.decisionView != null)
    {
      if(gameView.decisionView.decisions !== undefined && gameView.decisionView.decisions !== null)
      {
        this.decisions = [];
        
        gameView.decisionView.decisions.forEach(dec => {
          this.decisions.push(new Decision(dec.decisionId, dec.decisionMarkdownText))
        });
      }

      this.decisionView = gameView.decisionView 
      
    }
    else if(gameView.gameFinishedView != null)
    {
      
      if(gameView.gameFinishedView.phaseResults !== undefined && gameView.gameFinishedView.phaseResults !== null)
      {

        let convertedPhaseResults = [] as PhaseResult[];
        gameView.gameFinishedView.phaseResults.forEach((pr, index) => {
          convertedPhaseResults.push(new PhaseResult(index, pr.decisionMarkdownText, pr.explanationMarkdownText, pr.decisionImpacts))
        });

        gameView.gameFinishedView.phaseResults = convertedPhaseResults
      }

      this.gameFinishedView = gameView.gameFinishedView 
    }
    else if(gameView.missionViewWithRemainingTime != null)
    {
      this.missionViewWithRemainingTime = gameView.missionViewWithRemainingTime 
      
      this.missionMarkDownText = gameView.missionViewWithRemainingTime.markDownText
      this.remainingTime = gameView.missionViewWithRemainingTime.remainingTime

      //Disable buttons
      this.isMissionStarted = false
      this.missionButtonReadyDisabled = true
    }
    else if(gameView.missionView != null)
    {     
      this.missionView = gameView.missionView 
      this.missionMarkDownText = gameView.missionView.markDownText
      this.isMissionStarted = gameView.missionView.missionStarted
      this.missionButtonReadyDisabled = this.isMissionStarted
    }

    this.$forceUpdate()
  }

  private handleCurrentChange(val: Decision) {
        this.selectedDecision = val;
  }

  private async saveDecisionButtonClicked() {
    
    this.saveDecisionButtonLoading = true;
    
    if(this.selectedDecision !== null)
    {
      let saveDecisionResponse = await submitDecision(this.gameId, this.selectedDecision.decisionId)

      if(!saveDecisionResponse.success || saveDecisionResponse.response === undefined )
      {
        //TODO show error
      }
      else
      {
        this.selectedDecision = null
      }
    }
    
    this.saveDecisionButtonLoading = false
  }

  private backToOverview() {
        this.$router.push('/game/overview')
  }

  resetCurrentView() {
    this.initialMissionView = null
    this.decisionView= null
    this.gameFinishedView = null
    this.missionViewWithRemainingTime = null
    this.missionView = null
       
    this.remainingTime = 0;

    this.isMissionStarted = false;
    this.missionButtonReadyDisabled = false;
    this.initialMissionButtonDisabled = false;
  }

  deactivated() {
    console.log('Stop connection')
    this.connection.stop()
  }

  activated() {
    console.log('Start connection')
    this.connection.start()
  }
}
</script>

<style lang="scss" scoped>

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
.fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}

</style>
