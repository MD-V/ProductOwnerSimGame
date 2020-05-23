<template>
  <div>
    <h4>{{ $t('game.InitialMissionScreen') }}</h4>
    <el-divider></el-divider>
     <el-row>
       <div>
         {{ $t('game.roleText') }} <b>{{$t('game.role' + role )}}</b>
       </div>
      </el-row>
     <el-row>
       <div v-html="compiledMarkdown"></div>
      </el-row>
    <el-divider></el-divider>
    <el-row>
      <el-col>
        <el-button
          v-if="!isMissionStarted"
          v-loading="buttonLoading"
          :disabled="buttonDisabled"
          type="success"
          @click="readyClicked">
          {{ $t('game.ready') }}
        </el-button>
      </el-col>
      <el-col v-if="buttonDisabled">       
        {{ $t('game.waitingForOtherPlayers') }}
      </el-col>    
    </el-row>
    <el-row>   
      <el-col>
        <el-button
          v-if="isMissionStarted && role !== undefined && role === GameRole.ProductOwner"
          v-loading="buttonLoading"
          type="success"
          @click="phaseDoneClicked">
          {{ $t('game.phaseDone') }}
        </el-button>
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { MarkedOptions } from 'marked';
import marked from 'marked';
import { GameRole } from '@/models/models';
import { PassThrough } from 'stream';


@Component({
  name: 'MissionViewComponent',
  components: {
  },
})
export default class extends Vue {
  
  @Prop() markDownText!: string 
  @Prop() role!: GameRole 
  @Prop() buttonLoading: boolean = false
  @Prop() buttonDisabled: boolean = false
  @Prop() missionStarted: boolean = false
  
  get compiledMarkdown() {
    return marked(this.markDownText);
  }

  get isMissionStarted () {
    console.log(this.missionStarted)
    
    return this.missionStarted
  }

  private readyClicked()
  {
    this.$emit('readyButtonClicked')
  }

  private phaseDoneClicked()
  {
    this.$emit('phaseDoneClicked')
  }
}
</script>