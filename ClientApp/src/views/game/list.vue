<template>
  <div class="app-container">
    <el-table
      v-loading="listLoading"
      :data="list"
      border
      fit
      highlight-current-row
      style="width: 100%"
    >
      <el-table-column
        :min-width="25"
        align="center"
        :label="$t('game.id')"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.gameId }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :min-width="8"
        align="center"
        :label="$t('game.accessCode')"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.accessCode }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :min-width="12"
        align="center"
        :label="$t('game.state')"
      >
        <template slot-scope="scope">
          <el-tag :type="scope.row.state | gameStatusFilter">
            {{$t('game.' + scope.row.state) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column
        :min-width="5"
        align="center"
        :label="$t('game.players')"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.currentJoinedPlayers }} / {{ scope.row.gameVariantPlayerCount }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :min-width="30"
        align="center"
        :label="$t('game.variant')"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.gameVariantName }}</span>
        </template>
      </el-table-column>
      <el-table-column
        align="center"
        :label="$t('table.actions')"
        :min-width="10"
      >
        <template slot-scope="scope">
          <router-link :to="'/game/play/' + scope.row.gameId">
            <el-button
              v-if="isUser && (scope.row.state==='Started' || scope.row.state==='InitialMissionScreen' || scope.row.state==='Finished')"
              type="primary"
              size="mini"
              icon="el-icon-video-play"
            >
            </el-button>
          </router-link>
            <el-button
              v-if="isGameMaster && scope.row.state==='WaitingForPlayers'"
              v-loading="startGameLoading" 
              @click="startGame(scope.row.gameId)"
              :disabled="scope.row.currentJoinedPlayers!=scope.row.gameVariantPlayerCount"
              type="primary"
              size="mini"
              icon="el-icon-video-play"
            >
            </el-button>
            <el-button
              v-if="isGameMaster && (scope.row.state==='Started' || scope.row.state==='InitialMissionScreen')"
              v-loading="cancelGameLoading" 
              @click="cancelGame(scope.row.gameId)"
              type="primary"
              size="mini"
              icon="el-icon-close"
            >
            </el-button>
        </template>
      </el-table-column>
      
    </el-table>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { getGames, startGame, cancelGame } from '@/api/game'
import { Game } from '@/models/models'
import { UserModule } from '@/store/modules/user'

@Component({
  name: 'ArticleList',
  components: {
    UserModule
  }
})
export default class extends Vue {
  
  private list: Game[] = []
  private listLoading = true

  private startGameLoading = false
  private cancelGameLoading = false

  private isGameMaster = UserModule.roles.indexOf('gamemaster') > -1

  private isUser = UserModule.roles.indexOf('user') > -1

  created() {
    this.getList()
  }

  private async getList() {
    this.listLoading = true
    const gameResponse  = await getGames()

    if(gameResponse.success && gameResponse.response !== undefined)
    {
      this.list = gameResponse.response
    }

    // Just to simulate the time of the request
    setTimeout(() => {
      this.listLoading = false
    }, 0.5 * 1000)
  }

  private async startGame(gameId: string) {
    this.startGameLoading = true

    const startGameResponse  = await startGame(gameId)

    if(startGameResponse.success && startGameResponse.response !== undefined)
    {
      //Show error
    }
    else
    {
      //Show error
    }

    this.getList()

    // Just to simulate the time of the request
    setTimeout(() => {
      this.startGameLoading = false
    }, 0.5 * 1000)
  }

  private async cancelGame(gameId: string) {
    this.cancelGameLoading = true

    const cancelGameResponse  = await cancelGame(gameId)

    if(cancelGameResponse.success && cancelGameResponse.response !== undefined)
    {
      //Show error
    }
    else
    {
      //Show error
    }

    this.getList()

    // Just to simulate the time of the request
    setTimeout(() => {
      this.cancelGameLoading = false
    }, 0.5 * 1000)
  }

}
</script>

<style lang="scss" scoped>
.edit-input {
  padding-right: 100px;
}

.cancel-btn {
  position: absolute;
  right: 15px;
  top: 10px;
}
</style>
