from dataclasses import dataclass

@dataclass
class AuthenticationCredentials:
    email: str
    password: str
