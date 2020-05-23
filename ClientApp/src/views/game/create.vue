<template>
  <div class="createPost-container">
    <div class="createPost-main-container">
      <h2>{{ $t("game.create") }}</h2>
      <el-divider></el-divider>
      <PlayerCountDropdown
        @change="handlePlayerCountChange"
        :selectedPlayerCount="selectedPlayerCount"
        :playerCounts="playerCounts"
      />
      <h4>{{ $t("game.chooseVariant") }}</h4>
      <el-select v-model="selectedGameVariant" style="width:30.0rem;">
        <el-option
          v-for="item in availableGameVariants"
          :key="item.gameVariantId"
          :label="item.displayName"
          :value="item"
        />
      </el-select>
      <el-divider></el-divider>
      <el-button v-loading="loading" type="success" @click="submitForm">
        {{ $t("game.create") }}
      </el-button>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { getGameVariants } from "@/api/gamevariant";
import { createGame } from "@/api/game";
import { PlayerCountDropdown } from "./components";
import { GameVariant } from "../../models/models";

@Component({
  name: "CreateGame",
  components: {
    PlayerCountDropdown,
  },
})
export default class extends Vue {
  @Prop({ default: false }) private isEdit!: boolean;

  playerCounts = [3, 4, 5];

  selectedPlayerCount: number = this.playerCounts[0];

  @Prop() availableGameVariants: GameVariant[] = [];
  @Prop() selectedGameVariant!: GameVariant;

  private loading = false;

  created() {
    this.getGameVariants(this.selectedPlayerCount);
  }

  private async getGameVariants(playerCount: number) {
    try {
      const response = await getGameVariants(playerCount);

      if (response.success && response.response !== undefined) {
        this.availableGameVariants = response.response;
        this.selectedGameVariant = response.response[0] as GameVariant;
      } else {
        this.availableGameVariants = [];
      }
    } catch (err) {
      console.error(err);
    }
  }

  private async submitForm() {
    this.loading = true;

    if (this.selectedGameVariant !== undefined) {
      let creategameResponse = await createGame(
        this.selectedGameVariant.gameVariantId
      );

      if (
        creategameResponse.success &&
        creategameResponse.response !== undefined
      ) {
        console.log(creategameResponse.response.accessCode);
        // Move to show accesscode page
        this.$router.push({
          name: "showcode",
          params: { accessCode: creategameResponse.response.accessCode },
        });
      }
    }

    // Just to simulate the time of the request
    setTimeout(() => {
      this.loading = false;
    });
  }

  private handlePlayerCountChange(value: number) {
    //TODO get gamevariants
    this.selectedPlayerCount = value;
    this.getGameVariants(value);
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
