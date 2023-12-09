import { readFileSync } from 'node:fs'

function* directionIterator(dir) {
    let i = 0
    while (true) {
        yield dir[i] === 'L' ? 0 : 1
        i = i === dir.length - 1 ? 0 : i + 1
    }
}

const input = readFileSync('input.txt', 'utf-8')
    .split(/\r?\n/)
    .filter(l => l.length)

const [_, ...rest] = input

const nodeMap = rest.reduce((map, line) => {
    const [part1, part2] = line.split('=')
    const key = part1.trim()
    const match = part2.match(/\(([A-Z]+), ([A-Z]+)\)/)
    map[key] = { key, value: [match[1], match[2]] }
    return map
}, {})

function partOne() {
    const iterator = directionIterator(input[0])
    let node = nodeMap['AAA']
    let destination = 'ZZZ'
    let hops = 0

    while (node.key !== destination) {
        const idx = iterator.next().value
        node = nodeMap[node.value[idx]]
        hops++
    }

    return hops
}

console.log(partOne())
