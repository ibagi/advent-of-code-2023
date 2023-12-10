import { readFileSync } from 'node:fs'

function* differences(array) {
    for (let i = 1; i < array.length; i++) {
        yield array[i] - array[i - 1]
    }
}

function extrapolate(array, results) {
    if (!array.every(v => v === 0)) {
        const diff = Array.from(differences(array))
        results.push(diff[diff.length - 1])
        extrapolate(diff, results)
    }

    return results
}

function extrapolateNextValue(history) {
    return extrapolate(history, []).reduce((a,b) => a + b, history[history.length - 1])
}

function partOne(histories) {
    return histories.reduce((sum, history) => sum + extrapolateNextValue(history), 0)
}

function partTwo(histories) {
    return histories.reduce((sum, history) => sum + extrapolateNextValue([...history].reverse()), 0)
}

const input = readFileSync('input.txt', 'utf-8')
    .split(/\r?\n/)
    .map(l => l.split(' ').map(v => parseInt(v.trim())))

console.log(partOne(input))
console.log(partTwo(input))
