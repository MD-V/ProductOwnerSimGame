<template>
  <div class="createPost-container">
    <div class="createPost-main-container">
      <h2>{{ $t('game.created') }}</h2>
      <el-divider></el-divider>
      <el-row>
        {{ $t('game.codeText') }}
      </el-row>
      <el-row>
        <h3>{{ this.accessCode }}</h3>
      </el-row>
      <el-divider></el-divider>
      <el-row>
        <el-button
          v-loading="loading"
          type="success"
          @click="backToOverview">
          {{ $t('route.backToDashboard') }}
        </el-button>
      </el-row>
      </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component({
  name: 'CreateGame',
  components: {
  }
})
export default class extends Vue {
  @Prop() accessCode!: string

  private loading = false
 

  created() { 
    let code = this.$route.params.accessCode
    this.accessCode = code
  }

  private backToOverview() {
    
        this.loading = true
        
        this.$router.push('/game/overview')
        
        // Just to simulate the time of the request
        setTimeout(() => {
          this.loading = false
        })
  }

}
</script>

<style lang="scss" scoped>

.el-row {
    margin-bottom: 20px;
    &:last-child {
      margin-bottom: 0;
    }
  }

.createPost-container {
  position: relative;

  .createPost-main-container {
    padding: 40px 45px 20px 50px;
  }
}
</style>
