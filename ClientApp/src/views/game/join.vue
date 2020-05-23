<template>
  <div class="createPost-container">
    <div class="createPost-main-container">
      <h2>{{ $t("game.join") }}</h2>
      <el-divider></el-divider>    
      <el-row>
        {{ $t('game.enterAccessCodeText') }}
      </el-row>
      <el-form
      ref="postForm"
      :model="postForm"
      :rules="rules"
      class="form-container">     
        <el-form-item>
          <el-input
            v-model="postForm.accessCode"
            :rows="1"
            type="textarea"
            class="article-textarea"
            autosize
            :placeholder="$t('game.accessCode')"
          />
        </el-form-item>    
    </el-form>
      <el-divider></el-divider>
      <el-button v-loading="loading" type="success" @click="submitForm">
        {{ $t("game.join") }}
      </el-button>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { joinGame } from "@/api/game";
import { GameVariant, JoinGameRequest, defaultJoinGameRequest } from "../../models/models";
import { Form, Message } from 'element-ui'

@Component({
  name: "JoinGame",
  components: {
  },
})
export default class extends Vue {
  @Prop({ default: false }) private isEdit!: boolean;

  private validateRequire = (rule: any, value: string, callback: Function) => {
    if (value === '') {
      if (rule.field === 'imageURL') {
        this.$message({
          message: 'Upload cover image is required',
          type: 'error'
        })
      } else {
        this.$message({
          message: rule.field + ' is required',
          type: 'error'
        })
      }
      callback(new Error(rule.field + ' is required'))
    } else {
      callback()
    }
  }

  private loading = false;
  private postForm = Object.assign({}, defaultJoinGameRequest)
  
  private rules = {   
    title: [{ validator: this.validateRequire }]
  }

  private async submitForm() {
    this.loading = true;
    
    console.log(this.postForm);

    let joinGameResponse = await joinGame(this.postForm.accessCode)
    
    if (
        joinGameResponse.success &&
        joinGameResponse.response !== undefined
      ) {
        // Move to show accesscode page
        this.$router.push({
          name: "gameoverview"
        });
      }
      else
      {
        let errorText = this.$t("game.joinError").toString()
        
        Message({
        message: errorText,
        type: 'error',
        duration: 5 * 1000
        })
      }

    // Just to simulate the time of the request
    setTimeout(() => {
      this.loading = false;
    });
    
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
