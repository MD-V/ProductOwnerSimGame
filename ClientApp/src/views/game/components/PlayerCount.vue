<template>
  <div>
    <h4>{{ $t('game.switchPlayerCount') }}</h4>
    <el-radio-group v-model="switchPlayerCount">
      <el-radio-button  v-for="item in playerCounts" v-bind:key="item" v-bind:label="item" />
    </el-radio-group>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component({
  name: 'PlayerCountDropdown'
})
export default class extends Vue {
  
  @Prop() selectedPlayerCount!: number 
  @Prop() playerCounts!: number[]

  get switchPlayerCount() {
    return this.selectedPlayerCount
  }

  set switchPlayerCount(value) { 
      this.$emit('change', value)
  }
}
</script>