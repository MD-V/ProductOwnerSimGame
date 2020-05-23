<template>
  <div>
    {{salary}}
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'


@Component({
  name: 'TestComponent',
  components: {

  }
})
export default class extends Vue {
  
  @Prop({required: true}) salary: number = 0 

}
</script>