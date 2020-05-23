// Set utils function parseTime to filter
export { parseTime } from '@/utils'

export const gameStatusFilter = (status: string) => {
  const statusMap: { [key: string]: string } = {
    WaitingForPlayers: 'info',
    InitialMissionScreen : '',
    Started: '',
    Finished: 'success',
    Cancelled: 'danger',
  }
  return statusMap[status]
}

// Filter to uppercase the first character
export const uppercaseFirstChar = (str: string) => {
  return str.charAt(0).toUpperCase() + str.slice(1)
}
