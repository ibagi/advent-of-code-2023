use std::fs;

fn main() {
    println!("{}", solution("../input.txt"));
}

fn solution(input_path: &str) -> i32 {
    let contents = fs::read_to_string(input_path).expect("file not found!");
    let input = contents.split("\n").collect::<Vec<&str>>();

    input
        .into_iter()
        .map(|line| parse_code(line))
        .reduce(|a, b| a + b)
        .unwrap_or(0)
}

fn parse_code(input: &str) -> i32 {
    let tokens = vec![
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "one", "two", "three", "four", "five",
        "six", "seven", "eight", "nine",
    ];

    let matches = tokens
        .iter()
        .map(|t| (t, input.match_indices(t)))
        .flat_map(|(t, i)| i.map(move |x| (t, x)))
        .collect::<Vec<_>>();

    let first = matches
        .iter()
        .min_by_key(|a| a.1)
        .map(|m| parse_number(*m.0).unwrap())
        .unwrap();

    let second = matches
        .iter()
        .max_by_key(|a| a.1)
        .map(|m| parse_number(*m.0).unwrap())
        .unwrap();

    return first * 10 + second;
}

fn parse_number(num_like: &str) -> Option<i32> {
    let digit = num_like.parse().unwrap_or(-1);
    if digit > -1 {
        return Some(digit);
    }

    match num_like {
        "one" => Some(1),
        "two" => Some(2),
        "three" => Some(3),
        "four" => Some(4),
        "five" => Some(5),
        "six" => Some(6),
        "seven" => Some(7),
        "eight" => Some(8),
        "nine" => Some(9),
        _ => None,
    }
}
