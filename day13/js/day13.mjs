import { readFileSync } from 'node:fs'

const input = readFileSync('input.txt', 'utf8').split('\r\n')
const patterns = readPatterns(input)

console.log(partOne(patterns))
console.log(partTwo(patterns))

function partOne(p) {
    return p.map(i => findReflection(i, 0)).reduce((sum, i) => sum + i, 0)
}

function partTwo(p) {
    return p.map(i => findReflection(i, 1)).reduce((sum, i) => sum + i, 0)
}

function findReflection(pattern, expectedDiff) {
    return [findVerticalReflection(pattern, expectedDiff), findHorizontalReflection(pattern, expectedDiff)].find(Boolean)
}

function findHorizontalReflection(pattern, expectedDiff) {
    for (let x = 1; x < pattern[0].length; x++) {
        const diff = pattern.reduce((sum, line) => sum + reflect(line.substring(0, x), line.substring(x), expectedDiff), 0)
        if (diff === expectedDiff) {
            return 1 * x
        }
    }

    return null
}

function findVerticalReflection(pattern, expectedDiff) {
    for (let y = 1; y < pattern.length; y++) {
        const toReflect = []
        for (let x = 0; x < pattern[0].length; x++) {
            toReflect.push([verticalLine(pattern, 0, y, x), verticalLine(pattern, y, pattern.length, x)])
        }

        const diff = toReflect.reduce((sum, [top, bottom]) => sum + reflect(top, bottom, expectedDiff), 0)
        if (diff === expectedDiff) {
            return 100 * y
        }
    }

    return null
}

function reflect(a, b) {
    const reversed = reverse(a)
    let mismatches = 0
    for (let i = 0; i < reversed.length; i++) {
        if (b[i] && reversed[i] !== b[i]) {
            mismatches++
        }
    }

    return mismatches
}

function verticalLine(pattern, from, to, x) {
    const result = []
    for (let i = from; i < to; i++) {
        result.push(pattern[i][x])
    }
    return result.join()
}

function reverse(text) {
    return [...text].reverse().join('')
}

function readPatterns(contents) {
    const result = []
    let lines = []
    for (let i = 0; i < contents.length; i++) {
        const line = contents[i]
        if (line.length === 0) {
            result.push(lines)
            lines = []
        } else {
            lines.push(line)
        }

        if (i === contents.length - 1) {
            result.push(lines)
        }
    }

    return result
}
