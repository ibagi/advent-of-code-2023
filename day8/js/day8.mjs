import { readFileSync } from 'node:fs'

function* directionIterator(dir) {
    let i = 0
    while (true) {
        yield dir[i] === 'L' ? 0 : 1
        i = i === dir.length - 1 ? 0 : i + 1
    }
}

function walk(start, stopPredicate) {
    const iterator = directionIterator(input[0])
    let node = start
    let hops = 0

    while (!stopPredicate(node)) {
        const idx = iterator.next().value
        node = nodeMap[node.value[idx]]
        hops++
    }

    return hops
}

function lcm(a,b) {
    return (a * b) / gcd(a, b)
}

function gcd(a, b) {
    let res = a
    while (b > 0) {
        const tmp = res
        res= b
        b = tmp % b
    }
    return res
}

function partOne(map) {
    return walk(map['AAA'], n => n.key === 'ZZZ')
}

function partTwo(map) {
    const nodes = Object.values(map).filter(n => n.key.endsWith('A'))
    const hops = []

    for(const node of nodes) {
        hops.push(walk(node, n => n.key.endsWith('Z')))
    }

    return hops.reduce((a,b) => lcm(a,b), 1)
}

const input = readFileSync('input.txt', 'utf-8')
    .split(/\r?\n/)
    .filter(l => l.length)

const [_, ...rest] = input

const nodeMap = rest.reduce((map, line) => {
    const [part1, part2] = line.split('=')
    const key = part1.trim()
    const match = part2.match(/\(([0-9A-Z]+), ([0-9A-Z]+)\)/)
    map[key] = { key, value: [match[1], match[2]] }
    return map
}, {})

console.log(partOne(nodeMap))
console.log(partTwo(nodeMap))
