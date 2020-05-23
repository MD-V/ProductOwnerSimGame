<template>
  <div class="dashboard-container">
    <EditorDashboard />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import EditorDashboard from './editor/index.vue'

@Component({
  name: 'Dashboard',
  components: {
    EditorDashboard
  }
})
export default class extends Vue {
}
</script>
