function formatNumber(n: number): string {
  let str = n.toString();
  return str[1] ? str : '0' + str;
}

export function formatTime(date: Date): string {
	let year = date.getFullYear();
	let month = date.getMonth() + 1;
	let day = date.getDate();

	let hour = date.getHours();
	let minute = date.getMinutes();
	let second = date.getSeconds();

	return [year, month, day].map(formatNumber).join('/') + ' ' + [hour, minute, second].map(formatNumber).join(':');
}
