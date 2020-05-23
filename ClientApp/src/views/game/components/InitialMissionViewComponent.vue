<template>
  <div>
    <h4>{{ $t('game.InitialMissionScreen') }}</h4>
    <el-divider></el-divider>
     <el-row>
       <div>
         {{ $t('game.roleText') }} <b>{{role}}</b>
       </div>
      </el-row>
     <el-row>
       <div v-html="compiledMarkdown"></div>
      </el-row>
    <el-divider></el-divider>
    <el-row>
      <el-col>
        <el-button
          v-loading="buttonLoading"
          :disabled="buttonDisabled"
          type="success"
          @click="buttonClicked">
          {{ $t('game.ready') }}
        </el-button>
      </el-col>
      <el-col v-if="buttonDisabled">       
        {{ $t('game.waitingForOtherPlayers') }}
      </el-col>    
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { MarkedOptions } from 'marked';
import marked from 'marked';
import { GameRole } from '../../../models/models';


@Component({
  name: 'InitialMissionViewComponent',
  components: {

  }
})
export default class extends Vue {
  
  @Prop() markDownText!: string 
  @Prop() role!: GameRole 
  @Prop() buttonLoading: boolean = false
  @Prop() buttonDisabled: boolean = false
  
  get compiledMarkdown() {
    return marked(this.markDownText);
  }

  private buttonClicked()
  {
    this.$emit('readyButtonClicked')
  }

}
</script>