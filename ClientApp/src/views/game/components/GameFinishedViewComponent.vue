<template>
  <div>
    <h4>{{ $t('game.InitialMissionScreen') }}</h4>
    <el-divider></el-divider>
     <el-row>
       <div v-html="compiledMarkdown"></div>
      </el-row>
    <el-divider></el-divider>
    <el-row>
        <el-button
          v-loading="loading"
          type="success"
          @click="backToDashboard">
          {{ $t('route.backToDashboard') }}
        </el-button>
      </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { MarkedOptions } from 'marked';
import marked from 'marked';


@Component({
  name: 'GameFinishedViewComponent',
  components: {

  }
})
export default class extends Vue {
  
  @Prop() overallMarkDownText!: string 

  get compiledMarkdown() {
    return "";
  }
}
</script>