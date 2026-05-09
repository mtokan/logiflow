const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string;

if (!API_BASE_URL) {
    throw new Error("VITE_API_BASE_URL is not configured.");
}

export async function getJson<T>(path: string): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${path}`);

    if (!response.ok) {
        throw new Error(`GET ${path} failed with status ${response.status}`);
    }

    const data = await response.json()
    return data as T;
}

export async function postJson<TResponse, TBody = unknown>(
    path: string,
    body?: TBody,
): Promise<TResponse> {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        method: "POST",
        headers:
            body === undefined
                ? undefined
                : {
                    "Content-Type": "application/json",
                },
        body: body === undefined ? undefined : JSON.stringify(body),
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(
            `POST ${path} failed with status ${response.status}: ${errorText}`,
        );
    }

    const data = await response.json();
    return data as TResponse
}